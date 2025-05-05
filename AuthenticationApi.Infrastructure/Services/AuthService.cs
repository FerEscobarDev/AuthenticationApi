using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.Logout;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationApi.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> LoginAsync(LoginUserCommand command)
        {
            var user = await _context.Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == command.Email.ToLower());

            if (user is null || !_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, command.Password).Equals(PasswordVerificationResult.Success))
                throw new ApplicationException("Invalid email or password.");

            if (!user.EmailConfirmed)
                throw new ApplicationException("You must confirm your email before logging in.");

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenHash = Hash(refreshToken);
            var expiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7"));

            var newRefreshToken = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                ExpiresAt = expiresAt,
                UserId = user.Id 
            };

            _context.RefreshTokens.Add(newRefreshToken);

            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30") * 60
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenCommand command)
        {
            var refreshTokenHash = Hash(command.RefreshToken);

            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .Where(rt => rt.TokenHash == refreshTokenHash && !rt.Revoked)
                .OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync();

            if (storedToken is null)
                throw new ApplicationException("Invalid refresh token.");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new ApplicationException("Refresh token has expired.");

            storedToken.Revoked = true;

            var user = storedToken.User;

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var newHash = Hash(newRefreshToken);
            var expiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7"));

            var newStored = new RefreshToken
            {
                TokenHash = newHash,
                ExpiresAt = expiresAt,
                UserId = user.Id
            };

            _context.RefreshTokens.Add(newStored);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30") * 60
            };
        }

        public async Task RevokeRefreshTokenAsync(LogoutCommand command)
        {
            var refreshTokenHash = Hash(command.RefreshToken);

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == refreshTokenHash && !rt.Revoked);

            if (storedToken is null)
                throw new ApplicationException("Refresh token is invalid or already revoked.");

            storedToken.Revoked = true;

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmEmailAsync(ConfirmEmailCommand command)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

            try
            {
                var principal = tokenHandler.ValidateToken(command.Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out _);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ApplicationException("Invalid token.");

                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    throw new ApplicationException("User not found.");

                if (user.EmailConfirmed)
                    return; 

                user.EmailConfirmed = true;
                await _context.SaveChangesAsync();
            }
            catch (SecurityTokenException)
            {
                throw new ApplicationException("Invalid or expired confirmation token.");
            }
        }

        public string GenerateEmailConfirmationToken(UserDto user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ConfirmEmailTokenExpirationMinutes"] ?? "15"));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateAccessToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30"));

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        private static string Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

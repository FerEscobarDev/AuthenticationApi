using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.RefreshToken;
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

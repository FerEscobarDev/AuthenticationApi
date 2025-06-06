﻿using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.Logout;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces.Services;
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
using AuthenticationApi.Application.Commands.ResetPassword;
using AuthenticationApi.Application.Interfaces.Repository;
using OtpNet;

namespace AuthenticationApi.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        
        public AuthService(IUserRepository userRepository, IRefreshTokensRepository refreshTokensRepository, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokensRepository = refreshTokensRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(command.EmailOrUsername, cancellationToken);

            if (user is null || !_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, command.Password).Equals(PasswordVerificationResult.Success))
                throw new ApplicationException("Invalid email or password.");
            
            if (user.TwoFactorEnabled)
            {
                if (string.IsNullOrWhiteSpace(command.TwoFactorCode) && string.IsNullOrWhiteSpace(command.RecoveryCode))
                    throw new ApplicationException("Two-factor authentication is required.");

                var valid2Fa = false;

                if (!string.IsNullOrWhiteSpace(command.TwoFactorCode))
                {
                    var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecretKey!));
                    valid2Fa = totp.VerifyTotp(command.TwoFactorCode.Trim(), out _, new VerificationWindow(2, 2));
                }

                if (!valid2Fa && !string.IsNullOrWhiteSpace(command.RecoveryCode))
                {
                    var rawCodes = user.TwoFactorRecoveryCodes ?? "[]";

                    var recoveryCodes = rawCodes
                        .Trim('[', ']')
                        .Replace("\"", "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .ToList();

                    if (recoveryCodes.Contains(command.RecoveryCode.Trim()))
                    {
                        valid2Fa = true;

                        recoveryCodes.Remove(command.RecoveryCode.Trim());
                        user.TwoFactorRecoveryCodes = $"[{string.Join(",", recoveryCodes.Select(code => $"\"{code}\""))}]";

                        await _userRepository.SaveChangesAsync(cancellationToken);
                    }
                }


                if (!valid2Fa)
                    throw new ApplicationException("Invalid two-factor authentication code.");
            }

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

            await _refreshTokensRepository.AddAsync(newRefreshToken, cancellationToken);
            await _refreshTokensRepository.SaveChangesAsync(cancellationToken);

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30") * 60
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            var refreshTokenHash = Hash(command.RefreshToken);

            var storedToken = await _refreshTokensRepository.GetValidTokenWithUserAsync(refreshTokenHash, cancellationToken);

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

            await _refreshTokensRepository.AddAsync(newStored, cancellationToken);
            await _refreshTokensRepository.SaveChangesAsync(cancellationToken);

            return new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30") * 60
            };
        }

        public async Task RevokeRefreshTokenAsync(LogoutCommand command, CancellationToken cancellationToken)
        {
            var refreshTokenHash = Hash(command.RefreshToken);

            var storedToken = await _refreshTokensRepository.GetValidByHashAsync(refreshTokenHash, cancellationToken);

            if (storedToken is null)
                throw new ApplicationException("Refresh token is invalid or already revoked.");

            storedToken.Revoked = true;

            await _refreshTokensRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task ConfirmEmailAsync(ConfirmEmailCommand command, CancellationToken cancellationToken)
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

                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), cancellationToken);
                if (user is null)
                    throw new ApplicationException("User not found.");

                if (user.EmailConfirmed)
                    return; 

                user.EmailConfirmed = true;
                await _userRepository.SaveChangesAsync(cancellationToken);
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
        
        public string GeneratePasswordResetToken(UserDto userDto)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ResetPasswordTokenExpirationMinutes"] ?? "10"));
        
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userDto.Id.ToString())
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
        
        public async Task ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
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
                if (userId is null)
                    throw new ApplicationException("Invalid token.");

                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), cancellationToken);
                if (user is null)
                    throw new ApplicationException("User not found.");

                user.PasswordHash = _passwordHasher.HashPassword(user, command.NewPassword);
                await _userRepository.SaveChangesAsync(cancellationToken);
            }
            catch (SecurityTokenException)
            {
                throw new ApplicationException("Invalid or expired token.");
            }
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

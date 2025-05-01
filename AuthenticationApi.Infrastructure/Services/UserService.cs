using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Persistence.Context;
using AuthenticationApi.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using AuthenticationApi.Application.Commands.RegisterUser;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Application.Interfaces;

namespace AuthenticationApi.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserCommand command)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(user => user.Email.ToLower() == command.Email.ToLower());

            if (existing is not null)
                throw new ApplicationException("The user already has that email.");

            var user = new User
            {
                UserName = command.UserName,
                Email = command.Email,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, command.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}

using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Persistence.Context;
using AuthenticationApi.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using AuthenticationApi.Application.Commands.RegisterUser;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserCommand command)
        {
            var user = new User
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, command.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}

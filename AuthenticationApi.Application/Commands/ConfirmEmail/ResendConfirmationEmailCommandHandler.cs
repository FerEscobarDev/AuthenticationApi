using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Persistence;
using AuthenticationApi.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Application.Commands.ConfirmEmail
{
    public sealed class ResendConfirmationEmailCommandHandler
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthEmailSender _authEmailSender;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public ResendConfirmationEmailCommandHandler(
            IApplicationDbContext context,
            IAuthEmailSender authEmailSender,
            IAuthService authService,
            IConfiguration configuration)
        {
            _context = context;
            _authEmailSender = authEmailSender;
            _authService = authService;
            _configuration = configuration;
        }

        public async Task HandleAsync(ResendConfirmationEmailCommand command)
        {
            var user = await _context.Users
                .Where(u => u.Email == command.Email)
                .FirstOrDefaultAsync();

            if (user is null)
                throw new ApplicationException("User not found.");

            if (user.EmailConfirmed)
                throw new ApplicationException("Email is already confirmed.");

            var token = _authService.GenerateEmailConfirmationToken(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            });

            var link = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?token={token}";

            await _authEmailSender.SendConfirmationLinkAsync(
                new UserDto { Id = user.Id, Email = user.Email, UserName = user.UserName },
                user.Email,
                link
            );
        }
    }
}

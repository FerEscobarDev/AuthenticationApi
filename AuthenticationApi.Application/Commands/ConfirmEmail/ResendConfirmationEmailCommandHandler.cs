using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Persistence;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Application.Interfaces.Services;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Application.Commands.ConfirmEmail
{
    public sealed class ResendConfirmationEmailCommandHandler
    {
        private readonly IAuthEmailSender _authEmailSender;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IGetUserByEmailOrUsernameQueryHandler _getUserByEmailOrUsernameQueryHandler;
        private readonly IValidator<ResendConfirmationEmailCommand> _validator;

        public ResendConfirmationEmailCommandHandler(
            IAuthEmailSender authEmailSender,
            IAuthService authService,
            IConfiguration configuration,
            IGetUserByEmailOrUsernameQueryHandler getUserByEmailOrUsernameQueryHandler,
            IValidator<ResendConfirmationEmailCommand> validator)
        {
            _authEmailSender = authEmailSender;
            _authService = authService;
            _configuration = configuration;
            _getUserByEmailOrUsernameQueryHandler = getUserByEmailOrUsernameQueryHandler;           
            _validator = validator;
        }

        public async Task HandleAsync(ResendConfirmationEmailCommand command, CancellationToken cancellationToken = default)
        {
            var validated = await _validator.ValidateAsync(command, cancellationToken);
            if (!validated.IsValid)
                throw new ValidationException(validated.Errors);

            var user = await _getUserByEmailOrUsernameQueryHandler.HandleAsync(new GetUserByEmailOrUserNameQuery { EmailOrUserName = command.Email }, cancellationToken);
            
            if (user is null)
                throw new ApplicationException("Error getting user.");

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

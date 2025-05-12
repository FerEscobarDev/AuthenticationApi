using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Application.Commands.RegisterUser
{
    public sealed class RegisterUserCommandHandler
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IAuthEmailSender _authEmailSender;
        private readonly IConfiguration _configuration;
        private readonly IValidator<RegisterUserCommand> _validator;


        public RegisterUserCommandHandler(
            IUserService userService, 
            IAuthEmailSender authEmailSender, 
            IConfiguration configuration, 
            IAuthService authService, 
            IValidator<RegisterUserCommand> validator)
        {
            _userService = userService;
            _authEmailSender = authEmailSender;
            _configuration = configuration;
            _authService = authService;
            _validator = validator;
        }

        public async Task<UserDto> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
        {
            var validated = await _validator.ValidateAsync(command, cancellationToken);
            
            if (!validated.IsValid)
                throw new ValidationException(validated.Errors);
            
            var userDto = await _userService.RegisterUserAsync(command);

            var token = _authService.GenerateEmailConfirmationToken(userDto);
            var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?token={token}";

            await _authEmailSender.SendConfirmationLinkAsync(userDto, userDto.Email, confirmationLink);

            return userDto;
        }
    }
}

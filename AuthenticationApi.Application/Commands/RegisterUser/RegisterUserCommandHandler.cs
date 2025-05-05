using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Application.Commands.RegisterUser
{
    public sealed class RegisterUserCommandHandler
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IAuthEmailSender _authEmailSender;
        private readonly IConfiguration _configuration;


        public RegisterUserCommandHandler(IUserService userService, IAuthEmailSender authEmailSender, IConfiguration configuration, IAuthService authService)
        {
            _userService = userService;
            _authEmailSender = authEmailSender;
            _configuration = configuration;
            _authService = authService;
        }

        public async Task<UserDto> HandleAsync(RegisterUserCommand command)
        {
            var userDto = await _userService.RegisterUserAsync(command);

            var token = _authService.GenerateEmailConfirmationToken(userDto);
            var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?token={token}";

            await _authEmailSender.SendConfirmationLinkAsync(userDto, userDto.Email, confirmationLink);

            return userDto;
        }
    }
}

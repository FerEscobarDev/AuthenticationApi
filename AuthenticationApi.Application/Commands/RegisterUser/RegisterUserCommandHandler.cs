using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;

namespace AuthenticationApi.Application.Commands.RegisterUser
{
    public sealed class RegisterUserCommandHandler
    {
        private readonly IUserService _userService;

        public RegisterUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserDto> HandleAsync(RegisterUserCommand command)
        {
            return await _userService.RegisterUserAsync(command);
        }
    }
}

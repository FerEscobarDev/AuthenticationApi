using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Application.Commands.LoginUser
{
    public sealed class LoginUserCommandHandler
    {
        private readonly IAuthService _authService;

        public LoginUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResultDto> HandleAsync(LoginUserCommand command)
        {
            return await _authService.LoginAsync(command);
        }
    }
}

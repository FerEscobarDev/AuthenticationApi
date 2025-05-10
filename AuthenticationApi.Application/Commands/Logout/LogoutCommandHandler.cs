using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Application.Commands.Logout
{
    public sealed class LogoutCommandHandler
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task HandleAsync(LogoutCommand command)
        {
            await _authService.RevokeRefreshTokenAsync(command);
        }
    }
}

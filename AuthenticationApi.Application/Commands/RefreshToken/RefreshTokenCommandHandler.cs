using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces;

namespace AuthenticationApi.Application.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResultDto> HandleAsync(RefreshTokenCommand command)
        {
            return await _authService.RefreshTokenAsync(command);
        }
    }
}

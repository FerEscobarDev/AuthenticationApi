using AuthenticationApi.Application.Interfaces;

namespace AuthenticationApi.Application.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailCommandHandler
    {
        private readonly IAuthService _authService;

        public ConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task HandleAsync(ConfirmEmailCommand command)
        {
            await _authService.ConfirmEmailAsync(command);
        }
    }
}

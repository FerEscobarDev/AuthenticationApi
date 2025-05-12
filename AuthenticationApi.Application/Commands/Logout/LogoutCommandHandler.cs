using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Services;
using FluentValidation;

namespace AuthenticationApi.Application.Commands.Logout
{
    public sealed class LogoutCommandHandler
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LogoutCommand> _validator;

        public LogoutCommandHandler(IAuthService authService, IValidator<LogoutCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        public async Task HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
        {
            var validated = await _validator.ValidateAsync(command, cancellationToken);
            if (!validated.IsValid)
                throw new ValidationException(validated.Errors);
            
            await _authService.RevokeRefreshTokenAsync(command);
        }
    }
}

using AuthenticationApi.Application.Interfaces.Services;
using FluentValidation;

namespace AuthenticationApi.Application.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailCommandHandler
    {
        private readonly IAuthService _authService;
        private readonly IValidator<ConfirmEmailCommand> _validator;
        
        public ConfirmEmailCommandHandler(IAuthService authService, IValidator<ConfirmEmailCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        public async Task HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken = default)
        {
            var validated = await _validator.ValidateAsync(command, cancellationToken);
            if (!validated.IsValid)
                throw new ValidationException(validated.Errors);
            
            await _authService.ConfirmEmailAsync(command, cancellationToken);
        }
    }
}

using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces.Services;
using FluentValidation;

namespace AuthenticationApi.Application.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RefreshTokenCommand> _validator; 

        public RefreshTokenCommandHandler(IAuthService authService, IValidator<RefreshTokenCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        public async Task<AuthResultDto> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            var validated = await _validator.ValidateAsync(command, cancellationToken);
            if (!validated.IsValid)
                throw new ValidationException(validated.Errors);
            
            return await _authService.RefreshTokenAsync(command, cancellationToken);
        }
    }
}

using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces.Services;
using FluentValidation;

namespace AuthenticationApi.Application.Commands.LoginUser
{
    public sealed class LoginUserCommandHandler
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginUserCommand> _validator; 

        public LoginUserCommandHandler(IAuthService authService, IValidator<LoginUserCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        public async Task<AuthResultDto> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
        {
            var validated = await _validator.ValidateAsync(command, cancellationToken);
            if (!validated.IsValid)
                throw new ValidationException(validated.Errors);
            
            return await _authService.LoginAsync(command, cancellationToken);
        }
    }
}

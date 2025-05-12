using AuthenticationApi.Application.Interfaces.Services;
using FluentValidation;

namespace AuthenticationApi.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler
{
    private readonly IAuthService _authService;
    private readonly IValidator<ResetPasswordCommand> _validator;

    public ResetPasswordCommandHandler(IAuthService authService, IValidator<ResetPasswordCommand> validator)
    {
        _authService = authService;
        _validator = validator;   
    }

    public async Task HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var validated = await _validator.ValidateAsync(command, cancellationToken);
        if (!validated.IsValid)
            throw new ValidationException(validated.Errors);
        
        await _authService.ResetPasswordAsync(command);
    }
}
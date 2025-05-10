using AuthenticationApi.Application.Interfaces;

namespace AuthenticationApi.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task HandleAsync(ResetPasswordCommand command)
    {
        await _authService.ResetPasswordAsync(command);
    }
}
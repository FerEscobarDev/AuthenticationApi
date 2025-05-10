namespace AuthenticationApi.Application.Commands.ResetPassword;

public class ResetPasswordCommand
{
    public string Token { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
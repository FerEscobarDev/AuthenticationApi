namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class Enable2FaCommand
{
    public string UserId { get; set; } = default!;
    public string Code { get; set; } = default!;
}
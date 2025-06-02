namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class Disable2FaCommand
{
    public string Code { get; set; } = default!;
}

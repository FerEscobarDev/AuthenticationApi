namespace AuthenticationApi.Application.Commands.ConfirmEmail
{
    public sealed class ResendConfirmationEmailCommand
    {
        public string Email { get; set; } = default!;
    }
}

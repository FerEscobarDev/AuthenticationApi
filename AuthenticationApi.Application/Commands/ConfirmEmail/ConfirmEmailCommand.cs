namespace AuthenticationApi.Application.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailCommand
    {
        public string Token { get; set; } = default!;
    }
}

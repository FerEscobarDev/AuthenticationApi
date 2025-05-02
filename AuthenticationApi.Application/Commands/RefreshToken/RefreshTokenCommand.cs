namespace AuthenticationApi.Application.Commands.RefreshToken
{
    public sealed class RefreshTokenCommand
    {
        public string RefreshToken { get; set; } = default!;
    }
}

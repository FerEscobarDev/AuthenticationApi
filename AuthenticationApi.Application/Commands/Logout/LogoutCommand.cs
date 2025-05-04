namespace AuthenticationApi.Application.Commands.Logout
{
    public sealed class LogoutCommand
    {
        public string RefreshToken { get; set; } = default!;
    }
}

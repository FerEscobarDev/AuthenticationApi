namespace AuthenticationApi.Application.Commands.RegisterUser
{
    public sealed class RegisterUserCommand
    {
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}

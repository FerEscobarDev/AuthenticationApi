namespace AuthenticationApi.Application.Commands.LoginUser
{
    public class LoginUserCommand
    {
        public string EmailOrUsername { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}

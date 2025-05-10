namespace AuthenticationApi.Application.Commands.LoginUser
{
    public class LoginUserCommand
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
    }
}

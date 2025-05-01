using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.DTOs.Auth;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginUserCommand command);
    }
}

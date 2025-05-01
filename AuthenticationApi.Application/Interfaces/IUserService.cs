using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.DTOs;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(RegisterUserCommand command);
    }
}

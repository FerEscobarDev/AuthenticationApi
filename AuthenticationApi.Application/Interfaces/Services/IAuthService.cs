using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.Logout;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.Commands.ResetPassword;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.DTOs.Auth;

namespace AuthenticationApi.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginUserCommand command);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenCommand command);
        Task RevokeRefreshTokenAsync(LogoutCommand command);
        Task ConfirmEmailAsync(ConfirmEmailCommand command);
        string GenerateEmailConfirmationToken(UserDto user);
        string GeneratePasswordResetToken(UserDto userDto);
        Task ResetPasswordAsync(ResetPasswordCommand command);

    }
}

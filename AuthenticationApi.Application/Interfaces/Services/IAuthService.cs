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
        Task<AuthResultDto> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(LogoutCommand command, CancellationToken cancellationToken);
        Task ConfirmEmailAsync(ConfirmEmailCommand command, CancellationToken cancellationToken);
        string GenerateEmailConfirmationToken(UserDto user);
        string GeneratePasswordResetToken(UserDto userDto);
        Task ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken);
    }
}

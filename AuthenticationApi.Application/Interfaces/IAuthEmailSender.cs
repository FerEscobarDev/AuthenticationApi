using AuthenticationApi.Application.DTOs;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IAuthEmailSender
    {
        public Task SendConfirmationLinkAsync(UserDto userDto, string email, string confirmationLink);
        public Task SendPasswordResetCodeAsync(UserDto userDto, string email, string resetCode);
        public Task SendPasswordResetLinkAsync(UserDto userDto, string email, string resetLink);
    }
}

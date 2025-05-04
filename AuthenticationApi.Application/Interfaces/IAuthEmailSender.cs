using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IAuthEmailSender
    {
        public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink);
        public Task SendPasswordResetCodeAsync(User user, string email, string resetCode);
        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink);
    }
}

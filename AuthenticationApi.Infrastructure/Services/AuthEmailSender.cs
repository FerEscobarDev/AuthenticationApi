using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Infrastructure.Services
{
    public sealed class AuthEmailSender : IAuthEmailSender
    {
        private readonly IEmailService _emailService;

        public AuthEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            string subject = "Confirm your email";
            string body = $"<H1>Confirm your email</H1><p>Please confirm your email by clicking this link: <a href='{confirmationLink}'>link</a></p>";

            await _emailService.SendAsync(email, subject, body);
        }

        public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            string subject = "Reset your password";
            string body = $"<H1>Reser your password</H1><p>Your password reset code is: {resetCode}</p>";

            await _emailService.SendAsync(email, subject, body);
        }

        public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            string subject = "Reset your password";
            string body = $"<H1>Reser your password</H1><p>Please reset your password by clicking this link: <a href='{resetLink}'>link</a></p>";

            await _emailService.SendAsync(email, subject, body);
        }
    }
}

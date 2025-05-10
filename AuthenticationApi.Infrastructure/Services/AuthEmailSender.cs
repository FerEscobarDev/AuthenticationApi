using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Infrastructure.Services
{
    public sealed class AuthEmailSender : IAuthEmailSender
    {
        private readonly IEmailService _emailService;

        public AuthEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendConfirmationLinkAsync(UserDto userDto, string email, string confirmationLink)
        {
            string subject = "Confirm your email";
            var body = $@"
                <p>Hello {userDto.UserName},</p>
                <p>Click the link below to confirm your email address:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>This link will expire in 15 minutes.</p>
            ";

            await _emailService.SendAsync(email, subject, body);
        }

        public async Task SendPasswordResetCodeAsync(UserDto userDto, string email, string resetCode)
        {
            string subject = "Reset your password";
            string body = $"<H1>Reser your password</H1><p>Your password reset code is: {resetCode}</p>";

            await _emailService.SendAsync(email, subject, body);
        }

        public async Task SendPasswordResetLinkAsync(UserDto userDto, string email, string resetLink)
        {
            var subject = "Reset your password";
            var body = $@"
                <p>Hello {userDto.UserName},</p>
                <p>Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>This link will expire in 10 minutes.</p>
            ";

            await _emailService.SendAsync(email, subject, body);
        }
    }
}

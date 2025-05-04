using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using AuthenticationApi.Application.Interfaces;

namespace AuthenticationApi.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            //TODO: Revisar implementación Auth2.0
            //TODO: Implementar validación de errores para cerrar la conexión SMTP
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = $"{subject} {_config["EmailSettings:SenderName"]}";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            var host = _config["EmailSettings:Host"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var useSsl = bool.TryParse(_config["EmailSettings:UseSsl"], out var ssl) && ssl;

            var socketOption = useSsl
                ? MailKit.Security.SecureSocketOptions.Auto
                : MailKit.Security.SecureSocketOptions.None;

            await smtp.ConnectAsync(host, port, socketOption);

            var user = _config["EmailSettings:Username"];
            var pass = _config["EmailSettings:Password"];

            if (!string.IsNullOrWhiteSpace(user))
            {
                await smtp.AuthenticateAsync(user, pass);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

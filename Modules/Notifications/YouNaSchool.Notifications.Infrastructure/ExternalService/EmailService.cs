using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using YouNaSchool.Notifications.Domain.Interfaces.Services;
using YouNaSchool.Notifications.Infrastructure.Settings;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                //sending the Message of passwordResetLink
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, true);
                    client.Authenticate(_emailSettings.FromEmail, _emailSettings.Password);
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = $"{message}",
                        TextBody = "welcome",
                    };
                    var Message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };
                    Message.From.Add(new MailboxAddress("YouNa School PlatForm Team", _emailSettings.FromEmail));
                    Message.To.Add(new MailboxAddress("testing", email));
                    Message.Subject = subject ?? "Not Submitted";
                    await client.SendAsync(Message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}

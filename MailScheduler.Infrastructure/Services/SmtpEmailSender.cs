using MailScheduler.Application.Interfaces;
using MailScheduler.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace MailScheduler.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtp;

        public SmtpEmailSender(IOptions<SmtpSettings> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }

        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_smtp.UserName));
            msg.To.Add(MailboxAddress.Parse(recipient));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, _smtp.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_smtp.UserName, _smtp.Password);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }
    }
}

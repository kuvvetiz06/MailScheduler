// MailScheduler.Infrastructure/Services/SmtpEmailSender.cs
using MailScheduler.Application.Interfaces;
using MailScheduler.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


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
            var email = new MimeMessage();
            // Gönderen adı varsa kullan, yoksa userName
            var displayName = string.IsNullOrWhiteSpace(_smtp.DisplayName)
                ? _smtp.UserName
                : _smtp.DisplayName;
            email.From.Add(new MailboxAddress(displayName, _smtp.UserName));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = StripHtml(body)
            };
            email.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            // Development için (gerekirse)
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            try
            {
                var secureOption = _smtp.EnableSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;
                await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(_smtp.UserName, _smtp.Password);
                await client.SendAsync(email);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true);
            }
        }

        // Basit HTML stripper; dilerseniz Regex veya HtmlAgilityPack ile geliştirin
        private static string StripHtml(string html)
        {
            var array = new char[html.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var @let in html)
            {
                if (@let == '<')
                {
                    inside = true;
                    continue;
                }

                if (@let == '>')
                {
                    inside = false;
                    continue;
                }

                if (!inside)
                {
                    array[arrayIndex++] = @let;
                }
            }

            return new string(array, 0, arrayIndex);
        }
    }
}

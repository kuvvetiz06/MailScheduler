
using MailScheduler.Application.Interfaces;
using MailScheduler.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace MailScheduler.Infrastructure.Services
{
    //public class SmtpEmailSender : IEmailSender
    //{
    //    private readonly SmtpSettings _smtp;

    //    public SmtpEmailSender(IOptions<SmtpSettings> smtpOptions)
    //    {
    //        _smtp = smtpOptions.Value;
    //    }


    //    public async Task SendEmailAsync(string to, IEnumerable<string>? cc, string subject, string body)
    //    {
    //        var message = new MimeMessage();
    //        message.From.Add(new MailboxAddress(_smtp.DisplayName, _smtp.UserName));
    //        message.To.Add(MailboxAddress.Parse(to));

    //        if (cc != null)
    //        {
    //            foreach (var address in cc)
    //            {
    //                message.Cc.Add(MailboxAddress.Parse(address));
    //            }
    //        }

    //        message.Subject = subject;
    //        var builder = new BodyBuilder
    //        {
    //            HtmlBody = body,
    //            TextBody = StripHtml(body)
    //        };
    //        message.Body = builder.ToMessageBody();

    //        using var client = new SmtpClient();
    //        // Geliştirme aşamasında self-signed sertifikalara izin
    //        client.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;

    //        var secureSocket = _smtp.EnableSsl
    //            ? SecureSocketOptions.StartTls
    //            : SecureSocketOptions.Auto;

    //        await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocket);
    //        //await client.AuthenticateAsync(_smtp.UserName, _smtp.Password);
    //        await client.SendAsync(message);

    //        if (client.IsConnected)
    //            await client.DisconnectAsync(true);
    //    }


    //    private static string StripHtml(string html)
    //    {
    //        var array = new char[html.Length];
    //        var idx = 0;
    //        var inside = false;
    //        foreach (var ch in html)
    //        {
    //            if (ch == '<') { inside = true; continue; }
    //            if (ch == '>') { inside = false; continue; }
    //            if (!inside) array[idx++] = ch;
    //        }
    //        return new string(array, 0, idx);
    //    }
    //}


    #region Test Sender Code
    /// <summary>
    /// SMTP üzerinden e-posta gönderimi sağlayan servis.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtp;

        public SmtpEmailSender(IOptions<SmtpSettings> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }

        /// <inheritdoc />
        public async Task SendEmailAsync(string to, IEnumerable<string>? cc, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.DisplayName ?? _smtp.UserName, _smtp.UserName));
            message.To.Add(MailboxAddress.Parse(to));

            if (cc != null)
            {
                foreach (var address in cc)
                {
                    message.Cc.Add(MailboxAddress.Parse(address));
                }
            }

            message.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = StripHtml(body)
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            // Geliştirme aşamasında self-signed sertifikalara izin
            client.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;

            var secureSocket = _smtp.EnableSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;

            await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocket);
            await client.AuthenticateAsync(_smtp.UserName, _smtp.Password);
            await client.SendAsync(message);

            if (client.IsConnected)
                await client.DisconnectAsync(true);
        }

        private static string StripHtml(string html)
        {
            var array = new char[html.Length];
            var idx = 0;
            var inside = false;
            foreach (var ch in html)
            {
                if (ch == '<') { inside = true; continue; }
                if (ch == '>') { inside = false; continue; }
                if (!inside) array[idx++] = ch;
            }
            return new string(array, 0, idx);
        }
    }
    #endregion
}


namespace MailScheduler.Application.Interfaces
{
    /// <summary>
    /// SMTP, SendGrid veya başka bir altyapı ile e‐posta gönderimini soyutlar.
    /// </summary>
    public interface IEmailSender
    {
        /// <param name="recipient">Alıcının e‐posta adresi</param>
        /// <param name="subject">E‐posta başlığı</param>
        /// <param name="body">HTML veya düz metin içeriği</param>
        Task SendEmailAsync(string recipient, string subject, string body);
    }
}

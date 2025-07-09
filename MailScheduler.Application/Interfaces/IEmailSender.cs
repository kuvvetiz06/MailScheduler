
namespace MailScheduler.Application.Interfaces
{
    public interface IEmailSender
    {
        /// <summary>Temel to ve cc bilgisiyle mail gönderir.</summary>
        Task SendEmailAsync(
            string to,
            IEnumerable<string>? cc,
            string subject,
            string body);
    }
}

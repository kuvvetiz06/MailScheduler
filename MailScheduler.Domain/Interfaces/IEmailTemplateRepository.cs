using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Enums;


namespace MailScheduler.Domain.Interfaces
{
    public interface IEmailTemplateRepository
    {
        Task AddAsync(EmailTemplate template);
        Task UpdateAsync(EmailTemplate template);
        Task DeleteAsync(EmailTemplate template);
        Task<EmailTemplate> GetByTypeAsync(MailRecipientType type);
    }
}

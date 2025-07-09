using MailScheduler.Domain.Entities;


namespace MailScheduler.Domain.Interfaces
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate?> GetByNameAsync(string name);
        Task AddAsync(EmailTemplate template);
        Task UpdateAsync(EmailTemplate template);
        Task DeleteAsync(EmailTemplate template);
    }
}

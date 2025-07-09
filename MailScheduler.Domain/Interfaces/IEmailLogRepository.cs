using MailScheduler.Domain.Entities;


namespace MailScheduler.Domain.Interfaces
{
    public interface IEmailLogRepository
    {
        Task AddAsync(EmailLog log);
        Task<EmailLog?> GetByEmployeeWeekAsync(string identityId, DateTime periodStart, int mailTypeId);
    }
}

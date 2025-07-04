using MailScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

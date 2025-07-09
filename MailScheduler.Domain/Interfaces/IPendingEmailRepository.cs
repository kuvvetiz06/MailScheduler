using MailScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Interfaces
{
    public interface IPendingEmailRepository
    {
        Task AddAsync(PendingEmail email);
        Task<IEnumerable<PendingEmail>> GetPendingAsync();
        Task UpdateAsync(PendingEmail email);
    }
}

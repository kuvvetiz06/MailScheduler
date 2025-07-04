using MailScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Interfaces
{
    public interface IRecipientRepository
    {
        Task AddAsync(Recipient r);
        Task<IEnumerable<Recipient>> GetAllAsync();
    }
}

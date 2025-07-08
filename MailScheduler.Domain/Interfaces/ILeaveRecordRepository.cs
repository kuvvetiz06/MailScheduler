using MailScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Interfaces
{
    public interface ILeaveRecordRepository
    {
        Task<IEnumerable<LeaveRecord>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}

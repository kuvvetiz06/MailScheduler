using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Repositories
{
    public class LeaveRecordRepository : ILeaveRecordRepository
    {
        private readonly ApplicationDbContext _context;
        public LeaveRecordRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<LeaveRecord>> GetByDateRangeAsync(DateTime start, DateTime end) =>
            await _context.LeaveRecords
                .Where(l => l.Date >= start && l.Date <= end)
                .ToListAsync();
    }
}

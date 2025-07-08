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
    public class AttendanceRecordRepository : IAttendanceRecordRepository
    {
        private readonly ApplicationDbContext _context;
        public AttendanceRecordRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<AttendanceRecord>> GetByDateRangeAsync(DateTime start, DateTime end) =>
            await _context.AttendanceRecords
                .Where(a => a.Date >= start && a.Date <= end)
                .ToListAsync();
    }
}

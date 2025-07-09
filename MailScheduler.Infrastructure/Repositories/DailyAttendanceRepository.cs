using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace MailScheduler.Infrastructure.Repositories
{
    public class DailyAttendanceRepository : IDailyAttendanceRepository
    {
        private readonly ApplicationDbContext _context;
        public DailyAttendanceRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<DailyAttendance>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.DailyAttendances
                .Where(d => d.Date >= start.Date && d.Date <= end.Date)
                .ToListAsync();
        }
    }
}

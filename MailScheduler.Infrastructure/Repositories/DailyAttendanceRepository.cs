using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Persistence;
using MailScheduler.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace MailScheduler.Infrastructure.Repositories
{
    public class DailyAttendanceRepository : IDailyAttendanceRepository
    {
        private readonly AttendanceDbContext _context;
        private readonly HashSet<string> _ignoreIds;
        public DailyAttendanceRepository(AttendanceDbContext context, IOptions<AttendanceSettings> opts)
        {
            _context = context;
            _ignoreIds = opts.Value.IgnoreUserIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<IEnumerable<DailyAttendance>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.DailyAttendances.AsNoTracking().Where(d => d.Date >= start.Date && d.Date <= end.Date &&
            d.IsLeave == 0 && d.IsTourniquet == 0 && d.IsTravel == 0 && d.IsDigital == 0 &&
            d.UserMail != "" && d.UserMail != null && d.IdentityId != null && !_ignoreIds.Contains(d.IdentityId)).ToListAsync();

        }

    }
}

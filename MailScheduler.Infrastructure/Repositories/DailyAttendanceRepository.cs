using EFCore.BulkExtensions;
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
        private readonly ApplicationDbContext _ctx;
        private readonly HashSet<string> _ignoreIds;
        public DailyAttendanceRepository(AttendanceDbContext context, IOptions<AttendanceSettings> opts, ApplicationDbContext cnt)
        {
            _context = context;
            _ignoreIds = opts.Value.IgnoreUserIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            _ctx = cnt;
        }

        public async Task<IEnumerable<DailyAttendance>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var data = await _context.DailyAttendances.Where(d => d.Date >= start.Date && d.Date <= end.Date).AsNoTracking().ToListAsync();

            
            if (data.Any())
            {
                var snapshots = data.Select(d => new AttendanceSnapshot
                {
                    IdentityId = d.IdentityId,
                    FullName = d.FullName,
                    UserMail = d.UserMail,
                    ManagerMail = d.ManagerMail,
                    HRPartnerMail = d.HRPartnerMail,
                    WorkPlace = d.WorkPlace,
                    Date = d.Date,
                    IsLeave = d.IsLeave,
                    IsTourniquet = d.IsTourniquet,
                    IsTravel = d.IsTravel,
                    IsDigital = d.IsDigital,

                }).ToList();

                _ctx.ChangeTracker.AutoDetectChangesEnabled = false;
                var bulkConfig = new BulkConfig
                {
                    BatchSize = 5000,
                    BulkCopyTimeout = 300,
                    UseTempDB = true,
                    EnableStreaming = true
                };
                await _ctx.BulkInsertAsync(snapshots, bulkConfig);
                _ctx.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return data.Where(d => d.IsLeave == 0 && d.IsTourniquet == 0 && d.IsTravel == 0 && d.IsDigital == 0 &&
            d.UserMail != "" && d.UserMail != null && d.IdentityId != null && !_ignoreIds.Contains(d.IdentityId));
        }

    }
}

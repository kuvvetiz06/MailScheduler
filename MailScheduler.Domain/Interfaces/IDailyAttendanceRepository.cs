using MailScheduler.Domain.Entities;


namespace MailScheduler.Domain.Interfaces
{
    public interface IDailyAttendanceRepository
    {
        Task<IEnumerable<DailyAttendance>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}

using Hangfire;
using MailScheduler.Application.IJobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Common;
using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Enums;
using MailScheduler.Domain.Interfaces;


namespace MailScheduler.Infrastructure.Jobs
{
    public class SendAttendanceReminderJob : ISendAttendanceReminderJob
    {
        private readonly IDailyAttendanceRepository _dailyRepo;
        private readonly IEmailLogRepository _logRepo;
        private readonly IPendingEmailRepository _pendingRepo;
        private readonly IEmailSender _sender;

        // Kişi bazlı retry ayarları
        private const int MaxEmailAttempts = 3;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(5);

        public SendAttendanceReminderJob(
            IDailyAttendanceRepository dailyRepo,
            IEmailLogRepository logRepo,
            IPendingEmailRepository pendingRepo,
            IEmailSender sender)
        {
            _dailyRepo = dailyRepo;
            _logRepo = logRepo;
            _pendingRepo = pendingRepo;
            _sender = sender;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ExecuteAsync()
        {
            var (start, end) = WeekHelper.GetPreviousWorkWeek(DateTime.Today);
            var records = await _dailyRepo.GetByDateRangeAsync(start, end);
            var days = Enumerable.Range(0, 5).Select(i => start.AddDays(i));

            var employees = records.Select(r => r.IdentityId).Distinct();
            foreach (var id in employees)
            {
                foreach (var day in days)
                {
                    var rec = records.FirstOrDefault(r => r.IdentityId == id && r.Date.Date == day);
                    bool onLeave = rec?.IsLeave ?? false;
                    bool present = rec?.IsTourniquet ?? false;
                    bool traveling = rec?.IsTravel ?? false;

                    if (!onLeave && !present && !traveling)
                    {
                        var prevLog = await _logRepo.GetByEmployeeWeekAsync(id, start, (int)MailType.MissingAttendanceInitial);
                        var type = prevLog != null ? MailType.MissingAttendanceReminder : MailType.MissingAttendanceInitial;

                        var email = id + "@example.com";
                        var subject = "Haftalık Devamsızlık Hatırlatma";
                        var body = $"{start:dd.MM.yyyy} - {end:dd.MM.yyyy} arası {day:dd.MM.yyyy} günü devamsızlık yapmış olduğunuz tespit edilmiştir. İşbu devamsızlığınıza yönelik açıklamanızı sunmanız yahut ilgili izin formunu 5 iş günü içerisinde doldurmanız gerektiğini hatırlatırız.";

                        bool sent = false;
                        string error = null;

                        for (int attempt = 1; attempt <= MaxEmailAttempts; attempt++)
                        {
                            try
                            {
                                await _sender.SendEmailAsync(email, subject, body);
                                sent = true;
                                break;
                            }
                            catch (Exception ex)
                            {
                                error = ex.Message;
                                if (attempt < MaxEmailAttempts)
                                    await Task.Delay(RetryDelay);
                            }
                        }

                        if (!sent)
                        {
                            // Başarısız olan maili pending tablosuna ekle
                            var pending = new PendingEmail(email, subject, body);
                            await _pendingRepo.AddAsync(pending);
                        }

                        var logEntry = new EmailLog(
                            email,
                            type.ToString(),
                            success: sent,
                            errorMessage: sent ? null : error,
                            identityId: id,
                            mailTypeId: (int)type,
                            periodStart: start,
                            periodEnd: end);

                        await _logRepo.AddAsync(logEntry);
                    }
                }
            }
        }
    }
}

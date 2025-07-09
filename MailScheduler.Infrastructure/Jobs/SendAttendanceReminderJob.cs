using MailScheduler.Application.IJobs;
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
        private readonly MailScheduler.Application.Interfaces.IEmailSender _sender;

        public SendAttendanceReminderJob(
            IDailyAttendanceRepository dailyRepo,
            IEmailLogRepository logRepo,
            MailScheduler.Application.Interfaces.IEmailSender sender)
        {
            _dailyRepo = dailyRepo;
            _logRepo = logRepo;
            _sender = sender;
        }

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

                    if (!onLeave && !present)
                    {
                        var prevLog = await _logRepo.GetByEmployeeWeekAsync(id, start, (int)MailType.MissingAttendanceInitial);
                        var type = prevLog != null
                            ? MailType.MissingAttendanceReminder
                            : MailType.MissingAttendanceInitial;

                        // TODO: IdentityId'den gerçek e-posta adresini çek
                        var email = id + "@example.com";
                        var subject = "Haftalık Devamsızlık Hatırlatma";
                        var body = $"{start:dd.MM.yyyy} - {end:dd.MM.yyyy} arasında {day:dd.MM.yyyy} günü işe gelmediniz. Lütfen izin formu doldurun veya İK'yı bilgilendirin.";

                        await _sender.SendEmailAsync(email, subject, body);

                        var log = new EmailLog(
                            email,
                            type.ToString(),
                            success: true,
                            errorMessage: null,
                            identityId: id,
                            mailTypeId: (int)type,
                            periodStart: start,
                            periodEnd: end);

                        await _logRepo.AddAsync(log);
                    }
                }
            }
        }
    }
}

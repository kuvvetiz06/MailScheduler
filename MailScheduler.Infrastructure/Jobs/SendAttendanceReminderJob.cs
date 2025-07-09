using Hangfire;
using MailScheduler.Application.Jobs;
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
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly IEmailLogRepository _logRepo;
        private readonly IPendingEmailRepository _pendingRepo;
        private readonly IEmailSender _sender;

        public SendAttendanceReminderJob(
            IDailyAttendanceRepository dailyRepo,
            IEmailTemplateRepository templateRepo,
            IEmailLogRepository logRepo,
            IPendingEmailRepository pendingRepo,
            IEmailSender sender)
        {
            _dailyRepo = dailyRepo;
            _templateRepo = templateRepo;
            _logRepo = logRepo;
            _pendingRepo = pendingRepo;
            _sender = sender;
        }

        private static string FillTemplate(string template, DailyAttendance rec, DateTime start, DateTime end)
        {
            return template
                .Replace("{StartDate}", start.ToString("dd.MM.yyyy"))
                .Replace("{EndDate}", end.ToString("dd.MM.yyyy"))
                .Replace("{EmployeeName}", $"{rec.Name} {rec.Surname}");
        }

        public async Task ExecuteAsync()
        {
            var (start, end) = WeekHelper.GetPreviousWorkWeek(DateTime.Today);
            var records = await _dailyRepo.GetByDateRangeAsync(start, end);

            foreach (var rec in records)
            {
                if (!rec.IsLeave && !rec.IsTourniquet && !rec.IsTravel)
                {
                    var tplUser = await _templateRepo.GetByTypeAsync(MailRecipientType.User);
                    var tplMgr = await _templateRepo.GetByTypeAsync(MailRecipientType.Manager);
                    var tplHR = await _templateRepo.GetByTypeAsync(MailRecipientType.HRPartner);

                    var to = rec.UserMail;
                    var cc = new List<string> { rec.ManagerMail, rec.HRPartnerMail };

                    // User mail
                    var bodyUser = FillTemplate(tplUser.Body, rec, start, end);
                    bool sentUser = await SendWithPendingAsync(to, cc, tplUser.Subject, bodyUser);

                    // Manager mail (CC: UserMail, HRPartnerMail)
                    var bodyMgr = FillTemplate(tplMgr.Body, rec, start, end);
                    bool sentMgr = await SendWithPendingAsync(rec.ManagerMail, null, tplMgr.Subject, bodyMgr);

                    // HR Partner mail (CC: UserMail, ManagerMail)
                    var bodyHR = FillTemplate(tplHR.Body, rec, start, end);
                    bool sentHR = await SendWithPendingAsync(rec.HRPartnerMail, null, tplHR.Subject, bodyHR);

                    // Log kayıtları
                    await _logRepo.AddAsync(new EmailLog(rec.UserMail, MailRecipientType.User.ToString(), sentUser));
                    await _logRepo.AddAsync(new EmailLog(rec.ManagerMail, MailRecipientType.Manager.ToString(), sentMgr));
                    await _logRepo.AddAsync(new EmailLog(rec.HRPartnerMail, MailRecipientType.HRPartner.ToString(), sentHR));
                }
            }
        }

        private async Task<bool> SendWithPendingAsync(string to, IEnumerable<string>? cc, string subject, string body)
        {
            try
            {
                await _sender.SendEmailAsync(to, cc, subject, body);
                return true;
            }
            catch
            {
                var pending = new PendingEmail(to, cc, subject, body);
                await _pendingRepo.AddAsync(pending);
                return false;
            }
        }
    }
}

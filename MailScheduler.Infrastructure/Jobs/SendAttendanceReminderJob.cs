using Hangfire;
using MailScheduler.Application.Jobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Common;
using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Enums;
using MailScheduler.Domain.Interfaces;
using Microsoft.Extensions.Logging;


namespace MailScheduler.Infrastructure.Jobs
{
    public class SendAttendanceReminderJob : ISendAttendanceReminderJob
    {
        private readonly IDailyAttendanceRepository _dailyRepo;
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly IEmailLogRepository _logRepo;
        private readonly IPendingEmailRepository _pendingRepo;
        private readonly IEmailSender _sender;
        private readonly ILogger<SendAttendanceReminderJob> _logger;

        public SendAttendanceReminderJob(
            IDailyAttendanceRepository dailyRepo,
            IEmailTemplateRepository templateRepo,
            IEmailLogRepository logRepo,
            IPendingEmailRepository pendingRepo,
            IEmailSender sender,
            ILogger<SendAttendanceReminderJob> logger)
        {
            _dailyRepo = dailyRepo;
            _templateRepo = templateRepo;
            _logRepo = logRepo;
            _pendingRepo = pendingRepo;
            _sender = sender;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("SendAttendanceReminderJob started at {Time}", DateTime.UtcNow);
            var (start, end) = WeekHelper.GetPreviousWorkWeek(DateTime.Today);
            var records = await _dailyRepo.GetByDateRangeAsync(start, end);
            _logger.LogInformation("Fetched {Count} attendance records between {Start} and {End}", records.Count(), start, end);

            foreach (var rec in records)
            {
                if (!rec.IsLeave && !rec.IsTourniquet && !rec.IsTravel)
                {
                    _logger.LogInformation("Processing employee {EmployeeId} for date {Date}", rec.IdentityId, rec.Date);
                    var tplUser = await _templateRepo.GetByTypeAsync(MailRecipientType.User);
                    var tplMgr = await _templateRepo.GetByTypeAsync(MailRecipientType.Manager);
                    var tplHR = await _templateRepo.GetByTypeAsync(MailRecipientType.HRPartner);

                    string ReplacePlaceholders(string template) => template
                        .Replace("{StartDate}", start.ToString("dd.MM.yyyy"))
                        .Replace("{EndDate}", end.ToString("dd.MM.yyyy"))
                        .Replace("{EmployeeName}", $"{rec.Name} {rec.Surname}");

                    var to = rec.UserMail;
                    var ccUser = new List<string> { rec.ManagerMail, rec.HRPartnerMail };

                    bool userSent = await SendWithPendingAsync(
                        to,
                        ccUser,
                        tplUser.Subject,
                        ReplacePlaceholders(tplUser.Body));
                    _logger.LogInformation("User email to {Email} sent: {Sent}", to, userSent);

                    bool mgrSent = await SendWithPendingAsync(
                        rec.ManagerMail,
                        new[] { rec.UserMail, rec.HRPartnerMail },
                        tplMgr.Subject,
                        ReplacePlaceholders(tplMgr.Body));
                    _logger.LogInformation("Manager email to {Email} sent: {Sent}", rec.ManagerMail, mgrSent);

                    bool hrSent = await SendWithPendingAsync(
                        rec.HRPartnerMail,
                        new[] { rec.UserMail, rec.ManagerMail },
                        tplHR.Subject,
                        ReplacePlaceholders(tplHR.Body));
                    _logger.LogInformation("HRPartner email to {Email} sent: {Sent}", rec.HRPartnerMail, hrSent);

                    await _logRepo.AddAsync(new EmailLog(rec.UserMail, MailRecipientType.User.ToString(), userSent));
                    await _logRepo.AddAsync(new EmailLog(rec.ManagerMail, MailRecipientType.Manager.ToString(), mgrSent));
                    await _logRepo.AddAsync(new EmailLog(rec.HRPartnerMail, MailRecipientType.HRPartner.ToString(), hrSent));
                }
            }
            _logger.LogInformation("SendAttendanceReminderJob finished at {Time}", DateTime.UtcNow);
        }

        private async Task<bool> SendWithPendingAsync(string to, IEnumerable<string>? cc, string subject, string body)
        {
            try
            {
                await _sender.SendEmailAsync(to, cc, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Adding to PendingEmails.", to);
                var pending = new PendingEmail(to, cc, subject, body);
                await _pendingRepo.AddAsync(pending);
                return false;
            }
        }
    }
}

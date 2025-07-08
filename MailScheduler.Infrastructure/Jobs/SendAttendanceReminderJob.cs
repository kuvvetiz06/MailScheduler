using MailScheduler.Application.IJobs;
using MailScheduler.Domain.Common;
using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Enums;
using MailScheduler.Domain.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Jobs
{
    public class SendAttendanceReminderJob : ISendAttendanceReminderJob
    {
        private readonly IAttendanceRecordRepository _attRepo;
        private readonly ILeaveRecordRepository _leaveRepo;
        private readonly IEmailLogRepository _logRepo;
        private readonly MailScheduler.Application.Interfaces.IEmailSender _sender;

        public SendAttendanceReminderJob(
            IAttendanceRecordRepository attRepo,
            ILeaveRecordRepository leaveRepo,
            IEmailLogRepository logRepo,
            MailScheduler.Application.Interfaces.IEmailSender sender)
        {
            _attRepo = attRepo;
            _leaveRepo = leaveRepo;
            _logRepo = logRepo;
            _sender = sender;
        }

        public async Task ExecuteAsync()
        {
            var (start, end) = WeekHelper.GetPreviousWorkWeek(DateTime.Today);
            var attendance = await _attRepo.GetByDateRangeAsync(start, end);
            var leaves = await _leaveRepo.GetByDateRangeAsync(start, end);
            var days = Enumerable.Range(0, 5).Select(i => start.AddDays(i));

            var employees = attendance.Select(a => a.IdentityId).Distinct();
            foreach (var id in employees)
            {
                foreach (var day in days)
                {
                    bool onLeave = leaves.Any(l => l.IdentityId == id && l.Date == day);
                    bool present = attendance.Any(a => a.IdentityId == id && a.Date == day && a.IsTourniquet == 1);
                    if (!onLeave && !present)
                    {
                        var prev = await _logRepo.GetByEmployeeWeekAsync(id, start, (int)MailType.MissingAttendanceInitial);
                        var type = prev != null ? MailType.MissingAttendanceReminder : MailType.MissingAttendanceInitial;
                        var email = /* fetch employee mail by id */ "user@example.com";
                        var subject = "Haftalık Devamsızlık Hatırlatma";
                        var body = $"{start:dd.MM.yyyy} - {end:dd.MM.yyyy} dönemi için {day:dd.MM.yyyy} günü işe gelmediniz. Lütfen izin formu doldurun veya İK'yı bilgilendirin.";

                        await _sender.SendEmailAsync(email, subject, body);
                        var log = new EmailLog(email, type.ToString(), true, null, id, (int)type, start, end);
                        await _logRepo.AddAsync(log);
                    }
                }
            }
        }
    }
}

using MailScheduler.Application.Interfaces;
using MailScheduler.Application.Jobs;
using MailScheduler.Domain.Common;
using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Enums;
using MailScheduler.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Ocsp;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // 1) Geçen haftanın başlangıç ve bitiş tarihleri
            var (lastWeekStart, lastWeekend) = WeekHelper.GetPreviousWorkWeek(DateTime.Now);

            // 2) O tarih aralığındaki tüm attendance kayıtlarını çek
            var lastWeekRecords = await _dailyRepo.GetByDateRangeAsync(lastWeekStart, lastWeekend);
            _logger.LogInformation("Fetched {Count} attendance records between, for last week {Start} and {End}", lastWeekRecords.Count(), lastWeekStart, lastWeekend);

            // 3) Kullanıcı bazında grupla
            var lastWeekGroups = lastWeekRecords
                .Select(r => new
                {
                    r.IdentityId,
                    r.UserMail,
                    r.ManagerMail,
                    r.HRPartnerMail,
                    r.FullName,
                    r.WorkPlace
                }).Distinct().ToList();

            var lastWeekGroupsCount = lastWeekGroups.Count();


            int CenteredCount = lastWeekGroups.Count(x => x.WorkPlace == "Merkez");
            // 4) Her bir kullanıcı için tek mail at
            foreach (var grp in lastWeekGroups)
            {
                // Gönderim adresleri ve adı soyadı
                var userMail = grp.UserMail;
                var managerMail = grp.ManagerMail;
                var hrPartnerMail = grp.HRPartnerMail;
                var employeeName = grp.FullName;
                var workPlace = grp.WorkPlace;
                var identityId = grp.IdentityId;
                var fullName = grp.FullName;

                var missingDays = lastWeekRecords.Where(x => x.IdentityId == identityId).Select(x => x.Date).OrderBy(d => d).ToList();


                // 2) Şablondaki placeholder’ları dolduruyoruz
                string Fill(string template) => template
                    .Replace("{EmployeeName}", $"<b>{employeeName}</b>");
                //.Replace("{MissingDates}", string.Join(", ", missingDays.Select(d => d.ToString("dd.MM.yyyy"))));

                // 3) HTML tablosunu oluşturup [EmailBody] içine yerleştiren fonksiyon
                async Task<string> WrapHtml(string bodyTemplate, string templateFileName)
                {
                    // a) Düz metin gövde
                    var filledBody = Fill(bodyTemplate);
                    // Eksik günler listesi
                    var tableHtml = new StringBuilder();
                    tableHtml.AppendLine("<h4>Devamsızlık Tarihleri</h4>");
                    tableHtml.AppendLine("<table " +
                        "border='1' cellpadding='5' cellspacing='0' " +
                        "style='border-collapse:collapse; width:100%;'>");

                    // Sütun genişliklerini belirtiyoruz
                    tableHtml.AppendLine("<colgroup>");
                    tableHtml.AppendLine("  <col style='width:50px;'>");   // # sütunu
                    tableHtml.AppendLine("  <col style='width:150px;'>");  // Tarih sütunu
                    tableHtml.AppendLine("</colgroup>");

                    tableHtml.AppendLine("<tr>");
                    tableHtml.AppendLine("  <th style='text-align:center;'>#</th>");
                    tableHtml.AppendLine("  <th style='text-align:left;'>Tarih</th>");
                    tableHtml.AppendLine("</tr>");

                    for (int i = 0; i < missingDays.Count; i++)
                    {
                        tableHtml.AppendLine("<tr>");
                        tableHtml.AppendLine($"  <td style='text-align:center;'>{i + 1}</td>");
                        tableHtml.AppendLine($"  <td style='text-align:left;'>{missingDays[i]:dd.MM.yyyy}</td>");
                        tableHtml.AppendLine("</tr>");
                    }

                    tableHtml.AppendLine("</table>");

                    // c) Şablon dosyasını oku
                    var path = Path.Combine(
                        AppContext.BaseDirectory,
                        "Templates", "Email", templateFileName);
                    var htmlTpl = await File.ReadAllTextAsync(path);

                    // d) [EmailBody] placeholder’ını hem düz gövde + tablo ile değiştir
                    return htmlTpl.Replace(
                        "[EmailBody]",
                        $"{filledBody}<br/>{tableHtml}");
                }


                //Merkez çalışanı ise Personel'e mail at, değilse HRPartner'a mail at
                if (workPlace == "Merkez")
                {
                    // ---USER MAIL---
                    if (!string.IsNullOrWhiteSpace(userMail))
                    {

                        var userTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.User);
                        var messageUser = Fill(userTpl.Body);
                        var htmlUser = await WrapHtml(userTpl.Body, "UserTemplate.html");
                        bool userOk = await SendWithPendingAsync(
                            to: userMail,
                            message: messageUser,
                            cc: null,
                            subject: userTpl.Subject,
                            body: htmlUser,
                            mailType: MailRecipientType.User);
                        _logger.LogInformation("MailSendLog - User email to {Email} sent: {Success}", userMail, userOk);


                    }
                    else
                    {
                        _logger.LogWarning("UserMail is empty for {Employee} and {IdentityID}. Skipping User email.", employeeName, identityId);
                        await _logRepo.AddAsync(new EmailLog(fullName + " UserID :" + identityId, "Standard Absence Text", MailRecipientType.User.ToString(), false, "User Mail is not found !", MailRecipientType.User.GetHashCode()));
                    }

                }
                else
                {
                    // --- HRPARTNER MAIL ---
                    if (!string.IsNullOrWhiteSpace(hrPartnerMail))
                    {
                        var hrTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.HRPartner);
                        var messageHr = Fill(hrTpl.Body);
                        var htmlHr = await WrapHtml(hrTpl.Body, "HRPartnerTemplate.html");
                        bool hrOk = await SendWithPendingAsync(
                            to: hrPartnerMail,
                            message: messageHr,
                            cc: null,
                            subject: hrTpl.Subject,
                            body: htmlHr,
                            mailType: MailRecipientType.HRPartner);
                        _logger.LogInformation("MailSendLog - HRPartner email to {Email} sent: {Success}", hrPartnerMail, hrOk);
                    }
                    else
                    {
                        _logger.LogWarning("HRPartnerMail is empty for {Employee} and {IdentityID}. Skipping User HRPartner email.", employeeName, identityId);
                        await _logRepo.AddAsync(new EmailLog(fullName + " UserID :" + identityId, "Standard Absence Text", MailRecipientType.HRPartner.ToString(), false, "User HRPartner Mail is not found !", MailRecipientType.HRPartner.GetHashCode()));
                    }
                }


            }


            var (twoWeeksStart, twoWeeksEnd) = WeekHelper.GetPreviousWorkWeek(lastWeekStart);

            var twoWeekRecords = await _dailyRepo.GetByDateRangeAsync(twoWeeksStart, twoWeeksEnd);
            _logger.LogInformation("Fetched {Count} attendance records between, for two week ago {Start} and {End}", twoWeekRecords.Count(), twoWeeksStart, twoWeeksEnd);


            var twoWeekGroups = twoWeekRecords
                .Select(r => new
                {
                    r.IdentityId,
                    r.UserMail,
                    r.ManagerMail,
                    r.HRPartnerMail,
                    r.FullName,
                    r.WorkPlace
                }).Distinct().ToList();

            var twoWeekGroupsCount = twoWeekGroups.Count();

            foreach (var grp in twoWeekGroups)
            {
                // Gönderim adresleri ve adı soyadı
                var userMail = grp.UserMail;
                var managerMail = grp.ManagerMail;
                var hrPartnerMail = grp.HRPartnerMail;
                var employeeName = grp.FullName;
                var workPlace = grp.WorkPlace;
                var identityId = grp.IdentityId;
                var fullName = grp.FullName;

                var missingDays = lastWeekRecords.Where(x => x.IdentityId == identityId).Select(x => x.Date).OrderBy(d => d).ToList();

                               
                // 2) Şablondaki placeholder’ları dolduruyoruz
                string Fill(string template) => template.Replace("{EmployeeName}", $"<b>{employeeName}</b>");


                // 3) HTML tablosunu oluşturup [EmailBody] içine yerleştiren fonksiyon
                async Task<string> WrapHtml(string bodyTemplate, string templateFileName)
                {
                    // a) Düz metin gövde
                    var filledBody = Fill(bodyTemplate);
                    // Eksik günler listesi
                    var tableHtml = new StringBuilder();
                    tableHtml.AppendLine("<h4>Devamsızlık Tarihleri</h4>");
                    tableHtml.AppendLine("<table " +
                        "border='1' cellpadding='5' cellspacing='0' " +
                        "style='border-collapse:collapse; width:100%;'>");

                    // Sütun genişliklerini belirtiyoruz
                    tableHtml.AppendLine("<colgroup>");
                    tableHtml.AppendLine("  <col style='width:50px;'>");   // # sütunu
                    tableHtml.AppendLine("  <col style='width:150px;'>");  // Tarih sütunu
                    tableHtml.AppendLine("</colgroup>");

                    tableHtml.AppendLine("<tr>");
                    tableHtml.AppendLine("  <th style='text-align:center;'>#</th>");
                    tableHtml.AppendLine("  <th style='text-align:left;'>Tarih</th>");
                    tableHtml.AppendLine("</tr>");

                    for (int i = 0; i < missingDays.Count; i++)
                    {
                        tableHtml.AppendLine("<tr>");
                        tableHtml.AppendLine($"  <td style='text-align:center;'>{i + 1}</td>");
                        tableHtml.AppendLine($"  <td style='text-align:left;'>{missingDays[i]:dd.MM.yyyy}</td>");
                        tableHtml.AppendLine("</tr>");
                    }

                    tableHtml.AppendLine("</table>");

                    // c) Şablon dosyasını oku
                    var path = Path.Combine(
                        AppContext.BaseDirectory,
                        "Templates", "Email", templateFileName);
                    var htmlTpl = await File.ReadAllTextAsync(path);

                    // d) [EmailBody] placeholder’ını hem düz gövde + tablo ile değiştir
                    return htmlTpl.Replace(
                        "[EmailBody]",
                        $"{filledBody}<br/>{tableHtml}");
                }

                // --- MANAGER MAIL ---
                if (!string.IsNullOrWhiteSpace(managerMail))
                {

                    var mgrTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.Manager);
                    var messageMgr = Fill(mgrTpl.Body);
                    var htmlMgr = await WrapHtml(mgrTpl.Body, "ManagerTemplate.html");
                    bool mgrOk = await SendWithPendingAsync(
                        to: managerMail,
                        message: messageMgr,
                        cc: null,
                        subject: mgrTpl.Subject,
                        body: htmlMgr,
                        mailType: MailRecipientType.Manager);
                    _logger.LogInformation("MailSendLog - Manager email to {Email} sent: {Success}", managerMail, mgrOk);
                }
                else
                {
                    _logger.LogWarning("ManagerMail is empty for {Employee} and {IdentityID}. Skipping User Manager email.", employeeName, identityId);
                    await _logRepo.AddAsync(new EmailLog(fullName + " UserID :" + identityId, "Standard Absence Text", MailRecipientType.Manager.ToString(), false, "User Manager Mail is not found !", MailRecipientType.Manager.GetHashCode()));
                }



                // --- HRPARTNER MAIL ---
                if (!string.IsNullOrWhiteSpace(hrPartnerMail))
                {
                    var hrTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.HRPartner);
                    var messageHr = Fill(hrTpl.Body);
                    var htmlHr = await WrapHtml(hrTpl.Body, "HRPartnerTemplate.html");
                    bool hrOk = await SendWithPendingAsync(
                        to: hrPartnerMail,
                        message: messageHr,
                        cc: null,
                        subject: hrTpl.Subject,
                        body: htmlHr,
                        mailType: MailRecipientType.HRPartner);
                    _logger.LogInformation("MailSendLog - HRPartner email to {Email} sent: {Success}", hrPartnerMail, hrOk);
                }
                else
                {
                    _logger.LogWarning("HRPartnerMail is empty for {Employee} and {IdentityID}. Skipping User HRPartner email.", employeeName, identityId);
                    await _logRepo.AddAsync(new EmailLog(fullName + " UserID :" + identityId, "Standard Absence Text", MailRecipientType.HRPartner.ToString(), false, "User HRPartner Mail is not found !", MailRecipientType.HRPartner.GetHashCode()));
                }
            }


            _logger.LogInformation("SendAttendanceReminderJob finished at {Time}", DateTime.UtcNow);
        }


        //public async Task ExecuteAsync()
        //{
        //    _logger.LogInformation("SendAttendanceReminderJob started at {Time}", DateTime.UtcNow);
        //    var (start, end) = WeekHelper.GetPreviousWorkWeek(DateTime.Today);
        //    var records = await _dailyRepo.GetByDateRangeAsync(start, end);
        //    _logger.LogInformation("Fetched {Count} attendance records between {Start} and {End}",
        //                           records.Count(), start, end);


        //    // Helper: şablonu oku, body’i replace et
        //    async Task<List<string>> WrapInMessageAndHtml(string tplBody, string templateFileName, DailyAttendance rec)
        //    {
        //        List<string> bodys = new List<string>();

        //        string Fill(string template) => template
        //        .Replace("{StartDate}", start.ToString("dd.MM.yyyy"))
        //        .Replace("{EndDate}", end.ToString("dd.MM.yyyy"))
        //        .Replace("{EmployeeName}", $"<b>{rec.FullName}</b>");

        //        var rawBody = Fill(tplBody);
        //        var path = Path.Combine(AppContext.BaseDirectory, "Templates", "Email", templateFileName);
        //        var htmlTpl = await File.ReadAllTextAsync(path);
        //        bodys.Add(rawBody);
        //        bodys.Add(htmlTpl.Replace("[EmailBody]", rawBody));
        //        return bodys;


        //    }

        //    foreach (var rec in records)
        //    {

        //        // USER
        //        var userTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.User);
        //        var htmlUser = await WrapInMessageAndHtml(userTpl.Body, "UserTemplate.html", rec);
        //        bool userOk = await SendWithPendingAsync(
        //            to: rec.UserMail,
        //            message: htmlUser[0],
        //            cc: null,
        //            subject: userTpl.Subject,
        //            body: htmlUser[1],
        //            MailRecipientType.User);
        //        _logger.LogInformation("User email to {Email} sent: {Success}", rec.UserMail, userOk);

        //        // MANAGER
        //        var mgrTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.Manager);
        //        var htmlMgr = await WrapInMessageAndHtml(mgrTpl.Body, "ManagerTemplate.html", rec);
        //        bool mgrOk = await SendWithPendingAsync(
        //            to: rec.ManagerMail,
        //             message: htmlMgr[0],
        //            cc: null,
        //            subject: mgrTpl.Subject,
        //            body: htmlMgr[1],
        //            MailRecipientType.Manager);
        //        _logger.LogInformation("Manager email to {Email} sent: {Success}", rec.ManagerMail, mgrOk);

        //        // HRPARTNER
        //        var hrTpl = await _templateRepo.GetByTypeAsync(MailRecipientType.HRPartner);
        //        var htmlHr = await WrapInMessageAndHtml(hrTpl.Body, "HRPartnerTemplate.html", rec);
        //        bool hrOk = await SendWithPendingAsync(
        //            to: rec.HRPartnerMail,
        //            message: htmlHr[0],
        //            cc: null,
        //            subject: hrTpl.Subject,
        //            body: htmlHr[1],
        //            MailRecipientType.HRPartner);
        //        _logger.LogInformation("HRPartner email to {Email} sent: {Success}", rec.HRPartnerMail, hrOk);
        //    }

        //    _logger.LogInformation("SendAttendanceReminderJob finished at {Time}", DateTime.UtcNow);
        //}

        private async Task<bool> SendWithPendingAsync(string to, string message, IEnumerable<string>? cc, string subject, string body, MailRecipientType mailType)
        {

            try
            {
                await _sender.SendEmailAsync(to, cc, subject, body);
                await _logRepo.AddAsync(new EmailLog(to, message, mailType.ToString(), true, null, mailType.GetHashCode()));
                return true;
            }
            catch (Exception ex)
            {
                await _logRepo.AddAsync(new EmailLog(to, message, mailType.ToString(), false, ex.Message, mailType.GetHashCode()));
                _logger.LogError(ex, "Failed to send email to {Email}. Adding to PendingEmails.", to);
                var pending = new PendingEmail(to, cc, subject, body);
                await _pendingRepo.AddAsync(pending);
                return false;
            }
        }
    }
}

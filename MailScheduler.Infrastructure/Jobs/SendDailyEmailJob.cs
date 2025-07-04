using MailScheduler.Application.IJobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Jobs
{
    public class SendDailyEmailJob : ISendDailyEmailJob
    {
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly IEmailLogRepository _logRepo;
        private readonly IEmailSender _sender;
        private readonly IConfiguration _config;

        public SendDailyEmailJob(
            IEmailTemplateRepository templateRepo,
            IEmailLogRepository logRepo,
            IEmailSender sender)
        {
            _templateRepo = templateRepo;
            _logRepo = logRepo;
            _sender = sender;
        }

        public async Task ExecuteAsync()
        {
            // 1) Template’leri oku
            var templates = new[] { "TemplateA", "TemplateB", "TemplateC" };
            foreach (var name in templates)
            {
                var template = await _templateRepo.GetByNameAsync(name);
                if (template == null) continue;

                // 2) Alıcı listelerini DB’den çek (bunu kendi sorgunuza göre ekleyin)
                var recipients = /* ... */ new List<string>();

                foreach (var r in recipients)
                {
                    bool success = false;
                    string? error = null;
                    try
                    {
                        await _sender.SendEmailAsync(r, template.Subject, template.Body);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }

                    // 3) Log kaydı
                    var log = new EmailLog(r, name, success, error);
                    await _logRepo.AddAsync(log);
                }
            }
        }
    }
}

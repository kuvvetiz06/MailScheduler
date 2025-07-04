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
            // Örnek şablon isimleri
            var templateNames = new[] { "TemplateA", "TemplateB", "TemplateC" };
            foreach (var name in templateNames)
            {
                var template = await _templateRepo.GetByNameAsync(name);
                if (template == null)
                    continue;

                // TODO: Buraya gerçek alıcı sorgusunu ekleyin.
                var recipients = new List<string> { "user1@example.com", "user2@example.com" };

                foreach (var recipient in recipients)
                {
                    bool success = false;
                    string? error = null;
                    try
                    {
                        await _sender.SendEmailAsync(recipient, template.Subject, template.Body);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }

                    var log = new EmailLog(recipient, name, success, error);
                    await _logRepo.AddAsync(log);
                }
            }
        }
    }
}

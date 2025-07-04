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
        private readonly IRecipientRepository _recipientRepo;
        private readonly IEmailLogRepository _logRepo;
        private readonly IEmailSender _sender;

        public SendDailyEmailJob(
            IEmailTemplateRepository templateRepo,
            IRecipientRepository recipientRepo,
            IEmailLogRepository logRepo,
            IEmailSender sender)
        {
            _templateRepo = templateRepo;
            _recipientRepo = recipientRepo;
            _logRepo = logRepo;
            _sender = sender;
        }

        public async Task ExecuteAsync()
        {
            var templateNames = new[] { "TemplateA", "TemplateB", "TemplateC" };
            var recipients = await _recipientRepo.GetAllAsync();

            foreach (var name in templateNames)
            {
                var template = await _templateRepo.GetByNameAsync(name);
                if (template == null) continue;

                foreach (var r in recipients)
                {
                    bool success = false;
                    string? error = null;
                    try
                    {
                        await _sender.SendEmailAsync(r.EmailAddress, template.Subject, template.Body);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }

                    var log = new EmailLog(r.EmailAddress, name, success, error);
                    await _logRepo.AddAsync(log);
                }
            }
        }
    }
}

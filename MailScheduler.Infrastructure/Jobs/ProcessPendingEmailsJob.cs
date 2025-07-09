using Hangfire;
using MailScheduler.Application.Jobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Jobs
{
    public class ProcessPendingEmailsJob : IProcessPendingEmailsJob
    {
        private readonly IPendingEmailRepository _pendingRepo;
        private readonly IEmailSender _sender;

        public ProcessPendingEmailsJob(
            IPendingEmailRepository pendingRepo,
            IEmailSender sender)
        {
            _pendingRepo = pendingRepo;
            _sender = sender;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ExecuteAsync()
        {
            var now = DateTime.UtcNow;
            var pending = (await _pendingRepo.GetPendingAsync()).ToList();

            foreach (var email in pending)
            {
                bool success = false;
                try
                {
                    // to, cc, subject, body geçiliyor
                    await _sender.SendEmailAsync(email.Recipient, email.Cc, email.Subject, email.Body);
                    success = true;
                }
                catch { }

                email.RecordAttempt(success);
                await _pendingRepo.UpdateAsync(email);
            }

            // Eğer Cuma 23:00 son çalışmasındaysak, kalan pending email'leri başarısız olarak işaretle
            if (now.DayOfWeek == DayOfWeek.Friday && now.Hour == 23)
            {
                var remaining = (await _pendingRepo.GetPendingAsync()).Where(p => !p.IsSuccess).ToList();
                foreach (var email in remaining)
                {
                    email.MarkFailed();
                    await _pendingRepo.UpdateAsync(email);
                }
            }
        }
    }
}

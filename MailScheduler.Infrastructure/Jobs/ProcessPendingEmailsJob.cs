using Hangfire;
using MailScheduler.Application.Jobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MailScheduler.Infrastructure.Jobs
{
    public class ProcessPendingEmailsJob : IProcessPendingEmailsJob
    {
        private readonly IPendingEmailRepository _pendingRepo;
        private readonly IEmailSender _sender;
        private readonly ILogger<ProcessPendingEmailsJob> _logger;

        public ProcessPendingEmailsJob(
            IPendingEmailRepository pendingRepo,
            IEmailSender sender,
            ILogger<ProcessPendingEmailsJob> logger)
        {
            _pendingRepo = pendingRepo;
            _sender = sender;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ExecuteAsync()
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("ProcessPendingEmailsJob started at {Time}", now);

            var pending = (await _pendingRepo.GetPendingAsync()).ToList();
            _logger.LogInformation("Found {Count} pending emails", pending.Count);

            foreach (var email in pending)
            {
                try
                {
                    await _sender.SendEmailAsync(email.Recipient, email.Cc, email.Subject, email.Body);
                    email.RecordAttempt(true);
                    _logger.LogInformation("Successfully resent email to {Email}", email.Recipient);
                }
                catch (Exception ex)
                {
                    email.RecordAttempt(false);
                    _logger.LogWarning(ex, "Retry failed for email to {Email}. Attempt {AttemptCount}", email.Recipient, email.AttemptCount);
                }
                await _pendingRepo.UpdateAsync(email);
            }

            if (now.DayOfWeek == DayOfWeek.Friday && now.Hour == 23)
            {
                var remaining = (await _pendingRepo.GetPendingAsync()).Where(p => !p.IsSuccess).ToList();
                foreach (var email in remaining)
                {
                    email.MarkFailed();
                    await _pendingRepo.UpdateAsync(email);
                    _logger.LogInformation("Marked pending email to {Email} as failed after final retry", email.Recipient);
                }
            }

            _logger.LogInformation("ProcessPendingEmailsJob finished at {Time}", DateTime.UtcNow);
        }
    }
}

using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    public class EmailLog : BaseEntity
    {
        public string Recipient { get; private set; } = null!;
        public string TemplateName { get; private set; } = null!;
        public DateTime SentAt { get; private set; }
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }

        private EmailLog() : base() { }

        public EmailLog(string recipient, string templateName, bool success, string? errorMessage = null) : base()
        {
            Recipient = recipient;
            TemplateName = templateName;
            SentAt = DateTime.UtcNow;
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}

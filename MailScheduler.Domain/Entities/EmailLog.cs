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
        public string? Recipient { get; private set; } = string.Empty;
        public string? TemplateName { get; private set; } = null!;
        public DateTime SentAt { get; private set; }
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int? MailTypeId { get; private set; }
        public string? SentMessage { get; private set; } = string.Empty;


        private EmailLog() : base() { }

        public EmailLog(
            string recipient,
            string sentMessage,
            string templateName,            
            bool success,
            string? errorMessage = null,
            int? mailTypeId = null
            )
        {
            Recipient = recipient;
            TemplateName = templateName;
            SentAt = DateTime.UtcNow;
            Success = success;
            ErrorMessage = errorMessage;
            MailTypeId = mailTypeId;
            SentMessage = sentMessage;
        }

        public void SetAttendanceInfo(int mailTypeId, DateTime start, DateTime end)
        {
            MailTypeId = mailTypeId;
            UpdateModified();
        }
    }
}

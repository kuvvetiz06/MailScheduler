using MailScheduler.Domain.Common;
using MailScheduler.Domain.Enums;


namespace MailScheduler.Domain.Entities
{
    public class EmailTemplate : BaseEntity
    {
        public MailRecipientType RecipientType { get; private set; }
        public string Subject { get; private set; } = null!;
        public string Body { get; private set; } = null!;

        private EmailTemplate() { }

        public EmailTemplate(
            MailRecipientType recipientType,
            string subject,
            string body)
            : base()
        {
            RecipientType = recipientType;
            Subject = subject;
            Body = body;
        }
    }
}

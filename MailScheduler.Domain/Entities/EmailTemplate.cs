using MailScheduler.Domain.Common;


namespace MailScheduler.Domain.Entities
{
    public class EmailTemplate : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Subject { get; private set; } = null!;
        public string Body { get; private set; } = null!;

        private EmailTemplate() : base() { }

        public EmailTemplate(string name, string subject, string body) : base()
        {
            Name = name;
            Subject = subject;
            Body = body;
        }

        public void UpdateTemplate(string subject, string body)
        {
            Subject = subject;
            Body = body;
            UpdateModified();
        }
    }
}

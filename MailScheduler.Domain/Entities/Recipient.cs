using MailScheduler.Domain.Common;


namespace MailScheduler.Domain.Entities
{
    public class Recipient : BaseEntity
    {
        public string EmailAddress { get; private set; } = null!;
        public string? Name { get; private set; }

        private Recipient() : base() { }

        public Recipient(string emailAddress, string? name = null) : base()
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}

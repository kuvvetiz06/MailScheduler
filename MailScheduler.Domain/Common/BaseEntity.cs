using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public DateTime? ModifiedDate { get; protected set; }
        public bool IsDeleted { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void MarkDeleted()
        {
            IsDeleted = true;
            ModifiedDate = DateTime.UtcNow;
        }

        public void UpdateModified()
        {
            ModifiedDate = DateTime.UtcNow;
        }
    }
}

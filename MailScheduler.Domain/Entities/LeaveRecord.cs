using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    public class LeaveRecord : BaseEntity
    {
        public string IdentityId { get; private set; } = null!;
        public DateTime Date { get; private set; }
        public int IsLeave { get; private set; }

        private LeaveRecord() : base() { }

        public LeaveRecord(string identityId, DateTime date, int isLeave) : base()
        {
            IdentityId = identityId;
            Date = date.Date;
            IsLeave = isLeave;
        }
    }
}

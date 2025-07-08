using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    public class AttendanceRecord : BaseEntity
    {
        public string IdentityId { get; private set; } = null!;
        public DateTime Date { get; private set; }
        public int IsTourniquet { get; private set; }

        private AttendanceRecord() : base() { }

        public AttendanceRecord(string identityId, DateTime date, int isTourniquet) : base()
        {
            IdentityId = identityId;
            Date = date.Date;
            IsTourniquet = isTourniquet;
        }
    }
}

using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    public class DailyAttendance : BaseEntity
    {
        public string IdentityId { get; private set; } = null!;
        public DateTime Date { get; private set; }
        public bool IsTourniquet { get; private set; }
        public bool IsLeave { get; private set; }

        private DailyAttendance() : base() { }

        public DailyAttendance(string identityId, DateTime date, bool isTourniquet, bool isLeave) : base()
        {
            IdentityId = identityId;
            Date = date.Date;
            IsTourniquet = isTourniquet;
            IsLeave = isLeave;
        }

        public void UpdateStatus(bool isTourniquet, bool isLeave)
        {
            IsTourniquet = isTourniquet;
            IsLeave = isLeave;
            UpdateModified();
        }
    }
}

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
  
        public bool IsTravel { get; private set; }

        private DailyAttendance() { }

   
        public DailyAttendance(string identityId, DateTime date, bool isTourniquet, bool isLeave, bool isTravel)
            : base()
        {
            IdentityId = identityId;
            Date = date.Date;
            IsTourniquet = isTourniquet;
            IsLeave = isLeave;
            IsTravel = isTravel;
        }

 
        public void UpdateStatus(bool isTourniquet, bool isLeave, bool isTravel)
        {
            IsTourniquet = isTourniquet;
            IsLeave = isLeave;
            IsTravel = isTravel;
            UpdateModified();
        }
    }

}

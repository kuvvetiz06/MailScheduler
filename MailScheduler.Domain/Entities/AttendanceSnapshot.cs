using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    public class AttendanceSnapshot : BaseEntity
    {
        public string? IdentityId { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? UserMail { get; set; } = string.Empty;
        public string? ManagerMail { get; set; } = string.Empty;
        public string? HRPartnerMail { get; set; } = string.Empty;
        public string? WorkPlace { get; set; } = string.Empty;

        public DateTime Date { get; set; }
        public int IsTourniquet { get; set; }
        public int IsLeave { get; set; }
        public int IsTravel { get; set; }
        public int IsDigital { get; set; }
    }

}

using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    /// <summary>
    /// Günlük yoklama, izin ve seyahat durumlarını tutar, ayrıca iletişim bilgileri içerir.
    /// </summary>
    public class DailyAttendance : BaseEntity
    {
        // Çalışan bilgileri
        public string IdentityId { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string Surname { get; private set; } = null!;
        public string UserMail { get; private set; } = null!;
        public string ManagerMail { get; private set; } = null!;
        public string HRPartnerMail { get; private set; } = null!;

        // Durum alanları
        public DateTime Date { get; private set; }
        public bool IsTourniquet { get; private set; }
        public bool IsLeave { get; private set; }
        public bool IsTravel { get; private set; }

        private DailyAttendance() { }

        public DailyAttendance(
            string identityId,
            string name,
            string surname,
            string userMail,
            string managerMail,
            string hrPartnerMail,
            DateTime date,
            bool isTourniquet,
            bool isLeave,
            bool isTravel)
            : base()
        {
            IdentityId = identityId;
            Name = name;
            Surname = surname;
            UserMail = userMail;
            ManagerMail = managerMail;
            HRPartnerMail = hrPartnerMail;
            Date = date.Date;
            IsTourniquet = isTourniquet;
            IsLeave = isLeave;
            IsTravel = isTravel;
        }

        public void UpdateStatus(
            bool isTourniquet,
            bool isLeave,
            bool isTravel)
        {
            IsTourniquet = isTourniquet;
            IsLeave = isLeave;
            IsTravel = isTravel;
            UpdateModified();
        }
    }

}

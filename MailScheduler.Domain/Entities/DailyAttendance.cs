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
    public class DailyAttendance 
    {
        // Çalışan bilgileri
        public string? IdentityId { get; private set; } = string.Empty;
        public string? FullName { get; private set; } = string.Empty;
        public string? UserMail { get; private set; } = string.Empty;
        public string? ManagerMail { get; private set; } = string.Empty;
        public string? HRPartnerMail { get; private set; } = string.Empty;
        public string? WorkPlace { get; private set; } = string.Empty;

        // Durum alanları
        public DateTime Date { get; private set; }
        public int IsTourniquet { get; private set; }
        public int IsLeave { get; private set; }
        public int IsTravel { get; private set; }
        public int IsDigital { get; private set; }
        private DailyAttendance() { }

       

        
    }

}

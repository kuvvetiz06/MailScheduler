using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Common
{
    /// <summary>
    /// Hafta başlangıcı ve bitiş tarihlerini hesaplayan yardımcı sınıf.
    /// </summary>
    public static class WeekHelper
    {
        /// <summary>
        /// Verilen tarihe göre haftanın Pazartesi gününü döner.
        /// </summary>
        public static DateTime GetCurrentMonday(DateTime date)
        {
            // C# DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
            int offset = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
            if (offset < 0)
                offset += 7;
            return date.Date.AddDays(-offset);
        }

        /// <summary>
        /// Verilen tarihe göre bir önceki haftanın Pazartesi ve Cuma tarih aralığını döner.
        /// </summary>
        public static (DateTime Start, DateTime End) GetPreviousWorkWeek(DateTime date)
        {
            var thisMonday = GetCurrentMonday(date);
            var prevMonday = thisMonday.AddDays(-7);
            var prevFriday = prevMonday.AddDays(4);
            return (prevMonday, prevFriday);
        }
    }
}

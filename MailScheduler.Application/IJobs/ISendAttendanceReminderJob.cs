using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Application.IJobs
{
    /// <summary>
    /// Haftalık devamsızlık hatırlatma işi arayüzü.
    /// </summary>
    public interface ISendAttendanceReminderJob
    {
        /// <summary>
        /// Haftalık devamsızlık kontrolü yapar ve gerekirse mail gönderir.
        /// </summary>
        Task ExecuteAsync();
    }
}

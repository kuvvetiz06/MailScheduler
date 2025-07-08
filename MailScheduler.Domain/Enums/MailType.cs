using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Enums
{
    /// <summary>
    /// Gönderilen mail bildirim türleri.
    /// </summary>
    public enum MailType
    {
        /// <summary>İlk devamsızlık bildirimi.</summary>
        MissingAttendanceInitial = 1,
        /// <summary>Hatırlatma bildirimi.</summary>
        MissingAttendanceReminder = 2
    }
}

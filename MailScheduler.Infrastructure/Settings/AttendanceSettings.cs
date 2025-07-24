using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Settings
{
    public class AttendanceSettings
    {
        public List<string> IgnoreUserIds { get; set; } = new();
    }

}

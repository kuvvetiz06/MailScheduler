using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Application.IJobs
{
  
    public interface ISendAttendanceReminderJob
    {
      
        Task ExecuteAsync();
    }
}

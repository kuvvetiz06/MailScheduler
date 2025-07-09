using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Application.IJobs
{
    /// <summary>
    /// Pending email retry job interface.
    /// </summary>
    public interface IProcessPendingEmailsJob
    {
        Task ExecuteAsync();
    }
}

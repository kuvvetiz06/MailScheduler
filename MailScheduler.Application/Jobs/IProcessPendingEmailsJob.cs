using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Application.Jobs
{
    public interface IProcessPendingEmailsJob
    {
        Task ExecuteAsync();
    }
}

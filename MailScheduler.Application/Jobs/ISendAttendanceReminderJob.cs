
namespace MailScheduler.Application.Jobs
{
  
    public interface ISendAttendanceReminderJob
    {
      
        Task ExecuteAsync();
    }
}

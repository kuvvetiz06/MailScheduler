using Hangfire;
using MailScheduler.Application.Jobs;
using MailScheduler.Infrastructure.Extensions;
using MailScheduler.Infrastructure.Jobs;
using MailScheduler.Infrastructure.Settings;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Repositories;
using Serilog.Events;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // Günlük dönen dosyalara yaz, her gün yeni dosya, 7 gün sakla:
    .WriteTo.File("Logs/log-.txt",
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//Serilog’u host’a baðla:
builder.Host.UseSerilog();

builder.Services.AddControllers();

// Infrastructure: DbContext, repos and other services
builder.Services.AddInfrastructure(builder.Configuration);


// Add DailyAttendance repository and job
builder.Services.AddScoped<IDailyAttendanceRepository, DailyAttendanceRepository>();
builder.Services.AddScoped<ISendAttendanceReminderJob, SendAttendanceReminderJob>();

// Add  PendingEmail repository and job
builder.Services.AddScoped<IPendingEmailRepository, PendingEmailRepository>();
builder.Services.AddScoped<IProcessPendingEmailsJob, ProcessPendingEmailsJob>();

// SMTP Email settings and sender
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<MailScheduler.Application.Interfaces.IEmailSender, MailScheduler.Infrastructure.Services.SmtpEmailSender>();

// Hangfire configuration
builder.Services.AddHangfire(cfg => cfg.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();



var app = builder.Build();
app.MapGet("/", () => Results.Text("Mail Scheduler Services Is Running", "text/plain"))
   .WithName("ServiceStatus")
   .WithOpenApi(operation =>
   {
       operation.Summary = "Service Status";
       operation.Description = "All Mail Scheduler  Services Are Working - Try It Services Running!";
       return operation;
   });
// Configure Hangfire Dashboard and job
app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<ISendAttendanceReminderJob>(
    "attendance-reminder-job",
    job => job.ExecuteAsync(),
    Cron.Weekly(DayOfWeek.Friday, 15, 0));

// Her Cuma 19:00,20:00,21:00,22:00 ve 23:00'te çalýþacak UTC + 3
RecurringJob.AddOrUpdate<IProcessPendingEmailsJob>(
    "process-pending-emails-job",
    job => job.ExecuteAsync(),
    
    "0 16,17,18,19,20 * * FRI"); ;

app.MapControllers();
app.Run();


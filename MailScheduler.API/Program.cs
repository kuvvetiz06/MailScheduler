using Hangfire;
using MailScheduler.Application.IJobs;
using MailScheduler.Infrastructure.Extensions;
using MailScheduler.Infrastructure.Jobs;
using MailScheduler.Infrastructure.Settings;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Infrastructure: DbContext, repos and other services
builder.Services.AddInfrastructure(builder.Configuration);

// Add DailyAttendance repository and job
builder.Services.AddScoped<IDailyAttendanceRepository, DailyAttendanceRepository>();
builder.Services.AddScoped<ISendAttendanceReminderJob, SendAttendanceReminderJob>();

// SMTP Email settings and sender
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<MailScheduler.Application.Interfaces.IEmailSender, MailScheduler.Infrastructure.Services.SmtpEmailSender>();

// Hangfire configuration
builder.Services.AddHangfire(cfg => cfg.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure Hangfire Dashboard and job
app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<ISendAttendanceReminderJob>(
    "attendance-reminder-job",
    job => job.ExecuteAsync(),
    Cron.Weekly(DayOfWeek.Friday, 18, 0));

app.MapControllers();
app.Run();


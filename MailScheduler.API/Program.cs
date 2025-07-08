using Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using MailScheduler.Application.IJobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Infrastructure.Authorization;
using MailScheduler.Infrastructure.Extensions;
using MailScheduler.Infrastructure.Jobs;
using MailScheduler.Infrastructure.Services;
using MailScheduler.Infrastructure.Settings;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// 1) Altyapýyý, DbContext ve repo'larý ekle
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAttendanceRecordRepository, AttendanceRecordRepository>();
builder.Services.AddScoped<ILeaveRecordRepository, LeaveRecordRepository>();
builder.Services.AddScoped<ISendAttendanceReminderJob, SendAttendanceReminderJob>();
// 2) Hangfire servisi
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

// 3) SMTP EmailSender (MailKit tabanlý)
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<ISendDailyEmailJob, SendDailyEmailJob>();

var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
//    await seeder.SeedAsync();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// 4) Hangfire Dashboard ve Server middleware
// Hangfire Dashboard with Basic Auth
var hangfireUser = builder.Configuration["HangfireSettings:UserName"];
var hangfirePass = builder.Configuration["HangfireSettings:Password"];
var hangfireAppPath = builder.Configuration["HangfireSettings:BackToSiteURL"];
app.UseHangfireDashboard(
    pathMatch: "/hangfire",
    options: new DashboardOptions
    {
        DashboardTitle = "Mail Scheduler Job Services",
        AppPath = hangfireAppPath,
        Authorization = new[]
        {
            new HangfireBasicAuthFilter(hangfireUser!, hangfirePass!)
        }
    });
app.UseHangfireServer();

// 5) Recurring job tanýmý (her gün 18:00)
RecurringJob.AddOrUpdate<ISendDailyEmailJob>(
    "daily-mail-job",
    job => job.ExecuteAsync(),
    Cron.Daily(15, 0));

app.MapControllers();
app.Run();



app.Run();


using Hangfire;
using MailScheduler.Application.IJobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Infrastructure.Extensions;
using MailScheduler.Infrastructure.Jobs;
using MailScheduler.Infrastructure.Services;
using MailScheduler.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// 1) Altyapýyý, DbContext ve repo'larý ekle
builder.Services.AddInfrastructure(builder.Configuration);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// 4) Hangfire Dashboard ve Server middleware
app.UseHangfireDashboard();       // http://<host>/hangfire
app.UseHangfireServer();

// 5) Recurring job tanýmý (her gün 18:00)
RecurringJob.AddOrUpdate<ISendDailyEmailJob>(
    "daily-mail-job",
    job => job.ExecuteAsync(),
    Cron.Daily(18, 0));

app.MapControllers();
app.Run();



app.Run();


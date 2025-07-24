using MailScheduler.Application.Jobs;
using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Jobs;
using MailScheduler.Infrastructure.Persistence;
using MailScheduler.Infrastructure.Repositories;
using MailScheduler.Infrastructure.Settings;
using MailScheduler.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MailScheduler.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(connectionString, b =>
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddDbContext<AttendanceDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("AttendanceConnection")),
            ServiceLifetime.Scoped);


            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.Configure<AttendanceSettings>(configuration.GetSection("AttendanceSettings"));

            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();
            services.AddScoped<IDailyAttendanceRepository, DailyAttendanceRepository>();
            services.AddScoped<ISendAttendanceReminderJob, SendAttendanceReminderJob>();
            services.AddScoped<IPendingEmailRepository, PendingEmailRepository>();
            services.AddScoped<IProcessPendingEmailsJob, ProcessPendingEmailsJob>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            return services;
        }
    }
}

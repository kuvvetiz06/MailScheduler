using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Persistence;
using MailScheduler.Infrastructure.Repositories;
using MailScheduler.Infrastructure.Settings;
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

            services.Configure<AttendanceSettings>(configuration.GetSection("AttendanceSettings"));

            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();
            services.AddScoped<IDailyAttendanceRepository, DailyAttendanceRepository>();
            return services;
        }
    }
}

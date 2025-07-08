using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Data;
using MailScheduler.Infrastructure.Persistence;
using MailScheduler.Infrastructure.Repositories;
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

            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();
            services.AddScoped<IDataSeeder, DatabaseSeeder>();
            return services;
        }
    }
}

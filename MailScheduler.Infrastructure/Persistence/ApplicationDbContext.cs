using MailScheduler.Domain.Entities;
using MailScheduler.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
        public DbSet<EmailLog> EmailLogs { get; set; } = null!;
        public DbSet<DailyAttendance> DailyAttendances { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmailLog>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<DailyAttendance>().HasQueryFilter(d => !d.IsDeleted);

            modelBuilder.ApplyConfiguration(new EmailLogConfiguration());
            modelBuilder.ApplyConfiguration(new DailyAttendanceConfiguration());
        }
    }
}

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
        public DbSet<Recipient> Recipients { get; set; } = null!;
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
        public DbSet<LeaveRecord> LeaveRecords { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmailLog>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<AttendanceRecord>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<LeaveRecord>().HasQueryFilter(l => !l.IsDeleted);

            modelBuilder.ApplyConfiguration(new EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new EmailLogConfiguration());
            modelBuilder.ApplyConfiguration(new RecipientConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceRecordConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveRecordConfiguration());
        }
    }
}

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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft-delete global filters
            modelBuilder.Entity<EmailTemplate>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<EmailLog>().HasQueryFilter(e => !e.IsDeleted);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new EmailLogConfiguration());
        }
    }
}

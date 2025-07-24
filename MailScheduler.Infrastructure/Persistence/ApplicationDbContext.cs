using MailScheduler.Domain.Entities;
using MailScheduler.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;


namespace MailScheduler.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
        public DbSet<EmailLog> EmailLogs { get; set; } = null!;
        public DbSet<PendingEmail> PendingEmails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailTemplate>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<EmailLog>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<PendingEmail>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.ApplyConfiguration(new EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new EmailLogConfiguration());
            modelBuilder.ApplyConfiguration(new PendingEmailConfiguration());
        }
    }
}

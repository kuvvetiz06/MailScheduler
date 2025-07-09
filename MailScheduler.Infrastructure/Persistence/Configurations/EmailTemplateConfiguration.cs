using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace MailScheduler.Infrastructure.Persistence.Configurations
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            builder.ToTable("EmailTemplates");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.RecipientType)
                   .IsRequired();

            builder.Property(e => e.Subject)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(e => e.Body)
                   .IsRequired();
        }
    }
}

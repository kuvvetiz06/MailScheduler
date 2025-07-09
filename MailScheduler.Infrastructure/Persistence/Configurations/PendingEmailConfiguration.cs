using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Persistence.Configurations
{
    public class PendingEmailConfiguration : IEntityTypeConfiguration<PendingEmail>
    {
        public void Configure(EntityTypeBuilder<PendingEmail> builder)
        {
            builder.ToTable("PendingEmails");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Recipient).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Subject).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Body).IsRequired();
            builder.Property(p => p.AttemptCount).IsRequired();
            builder.Property(p => p.LastAttempt).IsRequired();
            builder.Property(p => p.IsSuccess).IsRequired();
            builder.Property(p => p.IsDeleted).IsRequired();
        }
    }
}

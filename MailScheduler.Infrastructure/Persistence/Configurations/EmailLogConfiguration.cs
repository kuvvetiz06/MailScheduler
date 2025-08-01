﻿using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace MailScheduler.Infrastructure.Persistence.Configurations
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.ToTable("EmailLogs");

            builder.Property(e => e.Recipient)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(e => e.TemplateName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.ErrorMessage)
                   .HasMaxLength(1000);

            builder.Property(e => e.SentMessage)
                   .HasColumnType("nvarchar(max)");
        }
    }
}

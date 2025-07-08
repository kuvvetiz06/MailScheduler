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
    public class LeaveRecordConfiguration : IEntityTypeConfiguration<LeaveRecord>
    {
        public void Configure(EntityTypeBuilder<LeaveRecord> builder)
        {
            builder.ToTable("LeaveRecords");
            builder.Property(l => l.IdentityId).HasMaxLength(50).IsRequired();
            builder.Property(l => l.Date).IsRequired();
            builder.Property(l => l.IsLeave).IsRequired();
        }
    }
}

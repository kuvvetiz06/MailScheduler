﻿using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace MailScheduler.Infrastructure.Persistence.Configurations
{
    public class DailyAttendanceConfiguration : IEntityTypeConfiguration<DailyAttendance>
    {
        public void Configure(EntityTypeBuilder<DailyAttendance> builder)
        {
            builder.ToTable("DailyAttendances");
            builder.Property(d => d.IdentityId);
            builder.Property(d => d.FullName);            
            builder.Property(d => d.UserMail);
            builder.Property(d => d.ManagerMail);
            builder.Property(d => d.HRPartnerMail);
            builder.Property(d => d.Date);
            builder.Property(d => d.IsTourniquet);
            builder.Property(d => d.IsLeave);
            builder.Property(d => d.IsTravel);
            builder.Property(d => d.IsDigital);
        }
    }
}

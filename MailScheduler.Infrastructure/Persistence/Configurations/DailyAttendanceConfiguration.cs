using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace MailScheduler.Infrastructure.Persistence.Configurations
{
    public class DailyAttendanceConfiguration : IEntityTypeConfiguration<DailyAttendance>
    {
        public void Configure(EntityTypeBuilder<DailyAttendance> builder)
        {
            builder.ToTable("DailyAttendances");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.IdentityId).HasMaxLength(50).IsRequired();
            builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
            builder.Property(d => d.Surname).HasMaxLength(100).IsRequired();
            builder.Property(d => d.UserMail).HasMaxLength(200).IsRequired();
            builder.Property(d => d.ManagerMail).HasMaxLength(200).IsRequired();
            builder.Property(d => d.HRPartnerMail).HasMaxLength(200).IsRequired();

            builder.Property(d => d.Date).IsRequired();
            builder.Property(d => d.IsTourniquet).IsRequired();
            builder.Property(d => d.IsLeave).IsRequired();
            builder.Property(d => d.IsTravel).IsRequired();
        }
    }
}

using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailScheduler.Infrastructure.Persistence.Configurations
{
    public class AttendanceSnapshotConfiguration
        : IEntityTypeConfiguration<AttendanceSnapshot>
    {
        public void Configure(EntityTypeBuilder<AttendanceSnapshot> builder)
        {
            // Tablo ve şema
            builder.ToTable("AttendanceSnapshots");

            builder.HasKey(x => x.Id);

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

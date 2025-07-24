using MailScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Persistence
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> opts)
            : base(opts) { }

        public DbSet<DailyAttendance> DailyAttendances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Eğer ek config’leriniz varsa buraya ekleyin (örneğin ValueObject’ler vs.)
            builder.Entity<DailyAttendance>(cfg =>
            {

                cfg.ToTable("finalized_datamart", schema: "metadata_ug");

                cfg.HasKey(e => e.IdentityId);

                cfg.Property(e => e.IdentityId)
                    .HasColumnName("user_id");

                cfg.Property(e => e.FullName)
                  .HasColumnName("full_name");

                cfg.Property(e => e.Date)
                  .HasColumnName("date");

                cfg.Property(e => e.UserMail)
                 .HasColumnName("email_address");

                cfg.Property(e => e.ManagerMail)
                 .HasColumnName("manager_email");

                cfg.Property(e => e.HRPartnerMail)
                .HasColumnName("hr_email");

                cfg.Property(e => e.WorkPlace)
                 .HasColumnName("workplace_tr");

                cfg.Property(e => e.IsTourniquet)
                .HasColumnName("turnike_flag");

                cfg.Property(e => e.IsLeave)
                .HasColumnName("izin_flag");

                cfg.Property(e => e.IsTravel)
                .HasColumnName("sgf_flag");

                cfg.Property(e => e.IsDigital)
               .HasColumnName("digital_iz_flag");
            });
        }
    }
}

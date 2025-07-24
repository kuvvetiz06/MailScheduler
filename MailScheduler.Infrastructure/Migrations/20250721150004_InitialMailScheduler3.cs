using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMailScheduler3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyAttendances");

            migrationBuilder.AlterColumn<string>(
                name: "SentMessage",
                table: "EmailLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SentMessage",
                table: "EmailLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "DailyAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HRPartnerMail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdentityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDigital = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsLeave = table.Column<bool>(type: "bit", nullable: false),
                    IsTourniquet = table.Column<bool>(type: "bit", nullable: false),
                    IsTravel = table.Column<bool>(type: "bit", nullable: false),
                    ManagerMail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserMail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyAttendances", x => x.Id);
                });
        }
    }
}

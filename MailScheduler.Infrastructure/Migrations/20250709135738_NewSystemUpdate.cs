using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewSystemUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "EmailTemplates");

            migrationBuilder.AddColumn<string>(
                name: "Cc",
                table: "PendingEmails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecipientType",
                table: "EmailTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HRPartnerMail",
                table: "DailyAttendances",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManagerMail",
                table: "DailyAttendances",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DailyAttendances",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "DailyAttendances",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserMail",
                table: "DailyAttendances",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cc",
                table: "PendingEmails");

            migrationBuilder.DropColumn(
                name: "RecipientType",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "HRPartnerMail",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "ManagerMail",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "UserMail",
                table: "DailyAttendances");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EmailTemplates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}

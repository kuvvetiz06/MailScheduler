using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DailyAttendanceEntityModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "EmailLogs");

            migrationBuilder.AddColumn<string>(
                name: "SentMessage",
                table: "EmailLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentMessage",
                table: "EmailLogs");

            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "EmailLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "EmailLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "EmailLogs",
                type: "datetime2",
                nullable: true);
        }
    }
}

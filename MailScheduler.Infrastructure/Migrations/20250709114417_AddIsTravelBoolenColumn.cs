using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsTravelBoolenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTravel",
                table: "DailyAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTravel",
                table: "DailyAttendances");
        }
    }
}

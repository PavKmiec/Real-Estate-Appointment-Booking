using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingWebsite.Data.Migrations
{
    public partial class addedIsCanceledBoolToAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCancelled",
                table: "Appointments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCancelled",
                table: "Appointments");
        }
    }
}

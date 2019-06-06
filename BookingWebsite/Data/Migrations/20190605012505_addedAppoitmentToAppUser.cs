using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingWebsite.Data.Migrations
{
    public partial class addedAppoitmentToAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AppointmentId",
                table: "AspNetUsers",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Appointments_AppointmentId",
                table: "AspNetUsers",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Appointments_AppointmentId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AppointmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "AspNetUsers");
        }
    }
}

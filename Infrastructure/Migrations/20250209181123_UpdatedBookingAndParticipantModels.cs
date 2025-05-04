using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBookingAndParticipantModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Trips",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ApplicationUserId",
                table: "Trips",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_AspNetUsers_ApplicationUserId",
                table: "Trips",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_AspNetUsers_ApplicationUserId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ApplicationUserId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Trips");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAllModelIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carrier",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "TripId",
                table: "Trips",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "Participants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ItineraryId",
                table: "Itineraries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Bookings",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Trips",
                newName: "TripId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Participants",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Itineraries",
                newName: "ItineraryId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bookings",
                newName: "BookingId");

            migrationBuilder.AddColumn<string>(
                name: "Carrier",
                table: "Payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Payments",
                type: "text",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBookingRoomAssignmentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_AspNetUsers_ApplicationUserId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Trips_TripId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Booking_BookingId",
                table: "Participants");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssignment_Participants_ParticipantId",
                table: "RoomAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAssignment",
                table: "RoomAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.RenameTable(
                name: "RoomAssignment",
                newName: "RoomAssignments");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAssignment_ParticipantId",
                table: "RoomAssignments",
                newName: "IX_RoomAssignments_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_TripId",
                table: "Bookings",
                newName: "IX_Bookings_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_ApplicationUserId",
                table: "Bookings",
                newName: "IX_Bookings_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAssignments",
                table: "RoomAssignments",
                column: "RoomAssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_ApplicationUserId",
                table: "Bookings",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Trips_TripId",
                table: "Bookings",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "TripId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Bookings_BookingId",
                table: "Participants",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssignments_Participants_ParticipantId",
                table: "RoomAssignments",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "ParticipantId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_ApplicationUserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Trips_TripId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Bookings_BookingId",
                table: "Participants");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssignments_Participants_ParticipantId",
                table: "RoomAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAssignments",
                table: "RoomAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "RoomAssignments",
                newName: "RoomAssignment");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAssignments_ParticipantId",
                table: "RoomAssignment",
                newName: "IX_RoomAssignment_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_TripId",
                table: "Booking",
                newName: "IX_Booking_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ApplicationUserId",
                table: "Booking",
                newName: "IX_Booking_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAssignment",
                table: "RoomAssignment",
                column: "RoomAssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Booking",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_AspNetUsers_ApplicationUserId",
                table: "Booking",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Trips_TripId",
                table: "Booking",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "TripId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Booking_BookingId",
                table: "Participants",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssignment_Participants_ParticipantId",
                table: "RoomAssignment",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "ParticipantId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

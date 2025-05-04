using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedNotificationPreferenceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotificationPreferences_ApplicationUserId",
                table: "NotificationPreferences");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_ApplicationUserId_Type",
                table: "NotificationPreferences",
                columns: new[] { "ApplicationUserId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotificationPreferences_ApplicationUserId_Type",
                table: "NotificationPreferences");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_ApplicationUserId",
                table: "NotificationPreferences",
                column: "ApplicationUserId");
        }
    }
}

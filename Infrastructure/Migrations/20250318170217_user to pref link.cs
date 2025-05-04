using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class usertopreflink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserNotificationPreferenceId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserNotificationPreferenceId",
                table: "AspNetUsers",
                column: "UserNotificationPreferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserNotifications_UserNotificationPreferenceId",
                table: "AspNetUsers",
                column: "UserNotificationPreferenceId",
                principalTable: "UserNotifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserNotifications_UserNotificationPreferenceId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserNotificationPreferenceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserNotificationPreferenceId",
                table: "AspNetUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renameuserNotificationstouserNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserNotifications_UserNotificationPreferenceId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_AspNetUsers_ApplicationUserId",
                table: "UserNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotifications",
                table: "UserNotifications");

            migrationBuilder.RenameTable(
                name: "UserNotifications",
                newName: "UserNotificationsPreferences");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotifications_ApplicationUserId",
                table: "UserNotificationsPreferences",
                newName: "IX_UserNotificationsPreferences_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotificationsPreferences",
                table: "UserNotificationsPreferences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserNotificationsPreferences_UserNotificationPr~",
                table: "AspNetUsers",
                column: "UserNotificationPreferenceId",
                principalTable: "UserNotificationsPreferences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationsPreferences_AspNetUsers_ApplicationUserId",
                table: "UserNotificationsPreferences",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserNotificationsPreferences_UserNotificationPr~",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationsPreferences_AspNetUsers_ApplicationUserId",
                table: "UserNotificationsPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotificationsPreferences",
                table: "UserNotificationsPreferences");

            migrationBuilder.RenameTable(
                name: "UserNotificationsPreferences",
                newName: "UserNotifications");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotificationsPreferences_ApplicationUserId",
                table: "UserNotifications",
                newName: "IX_UserNotifications_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotifications",
                table: "UserNotifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserNotifications_UserNotificationPreferenceId",
                table: "AspNetUsers",
                column: "UserNotificationPreferenceId",
                principalTable: "UserNotifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_AspNetUsers_ApplicationUserId",
                table: "UserNotifications",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

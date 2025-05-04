using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotificationModelAndSomeUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isFeatured",
                table: "Trips",
                newName: "IsFeatured");

            migrationBuilder.RenameColumn(
                name: "isRead",
                table: "ContactMessages",
                newName: "IsRead");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFeatured",
                table: "Trips",
                newName: "isFeatured");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "ContactMessages",
                newName: "isRead");
        }
    }
}

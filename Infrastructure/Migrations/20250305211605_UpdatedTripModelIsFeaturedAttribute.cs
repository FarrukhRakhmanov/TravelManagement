using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTripModelIsFeaturedAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Featured",
                table: "Trips",
                newName: "isFeatured");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isFeatured",
                table: "Trips",
                newName: "Featured");
        }
    }
}

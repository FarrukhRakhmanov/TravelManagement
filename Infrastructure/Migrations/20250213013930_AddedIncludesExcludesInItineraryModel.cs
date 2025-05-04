using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedIncludesExcludesInItineraryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Excludes",
                table: "Itineraries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Includes",
                table: "Itineraries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Excludes",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "Includes",
                table: "Itineraries");
        }
    }
}

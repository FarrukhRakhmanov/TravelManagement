using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTripStyleEnumToTripModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "OriginalPricePerPerson",
                table: "Trips",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<int>(
                name: "Style",
                table: "Trips",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Style",
                table: "Trips");

            migrationBuilder.AlterColumn<double>(
                name: "OriginalPricePerPerson",
                table: "Trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}

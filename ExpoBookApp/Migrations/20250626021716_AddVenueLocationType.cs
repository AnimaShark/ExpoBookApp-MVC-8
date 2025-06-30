using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpoBookApp.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueLocationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationType",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationType",
                table: "Venues");
        }
    }
}

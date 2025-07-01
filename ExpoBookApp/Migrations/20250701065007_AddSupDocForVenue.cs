using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpoBookApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSupDocForVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportingDocumentPath",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportingDocumentPath",
                table: "Venues");
        }
    }
}

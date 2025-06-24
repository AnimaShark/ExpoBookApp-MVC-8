using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpoBookApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameThemeToEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Theme",
                table: "Events",
                newName: "EventType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "Events",
                newName: "Theme");
        }
    }
}

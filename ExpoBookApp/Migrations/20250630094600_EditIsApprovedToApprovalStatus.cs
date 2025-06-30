using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpoBookApp.Migrations
{
    /// <inheritdoc />
    public partial class EditIsApprovedToApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Venues");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Venues");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Venues",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

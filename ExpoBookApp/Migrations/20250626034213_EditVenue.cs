using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpoBookApp.Migrations
{
    /// <inheritdoc />
    public partial class EditVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "LocationSize",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "PricePerDay",
                table: "Venues");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Venues",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "LocationType",
                table: "Venues",
                newName: "StateCode");

            migrationBuilder.RenameColumn(
                name: "LocationAddress",
                table: "Venues",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Venues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "StateCode",
                table: "Venues",
                newName: "LocationType");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Venues",
                newName: "LocationAddress");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Venues",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Venues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Venues",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Venues",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LocationSize",
                table: "Venues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerDay",
                table: "Venues",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

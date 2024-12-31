using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedEventABBRelectioninfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionInfoCreatedAt",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "ElectionInfoDeletedAt",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "ElectionInfoUpdatedAt",
                table: "ElectionInfoMaster");

            migrationBuilder.RenameColumn(
                name: "SOUserId",
                table: "ElectionInfoMaster",
                newName: "FOUserId");

            migrationBuilder.RenameColumn(
                name: "PCMasterId",
                table: "ElectionInfoMaster",
                newName: "PSZoneMasterId");

            migrationBuilder.AddColumn<string>(
                name: "EventABBR",
                table: "ElectionInfoMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventSequence",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FourthLevelMasterId",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventABBR",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "EventSequence",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "FourthLevelMasterId",
                table: "ElectionInfoMaster");

            migrationBuilder.RenameColumn(
                name: "PSZoneMasterId",
                table: "ElectionInfoMaster",
                newName: "PCMasterId");

            migrationBuilder.RenameColumn(
                name: "FOUserId",
                table: "ElectionInfoMaster",
                newName: "SOUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ElectionInfoCreatedAt",
                table: "ElectionInfoMaster",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ElectionInfoDeletedAt",
                table: "ElectionInfoMaster",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ElectionInfoUpdatedAt",
                table: "ElectionInfoMaster",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}

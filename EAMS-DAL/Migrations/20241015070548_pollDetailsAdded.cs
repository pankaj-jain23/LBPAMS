using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class pollDetailsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VotesPolled",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VotesPolledLastUpdate",
                table: "ElectionInfoMaster",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VotesPolled",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "VotesPolledLastUpdate",
                table: "ElectionInfoMaster");
        }
    }
}

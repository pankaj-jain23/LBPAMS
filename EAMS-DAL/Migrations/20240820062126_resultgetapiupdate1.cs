using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class resultgetapiupdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultDecUpdatedAt",
                table: "Result");

            migrationBuilder.AddColumn<string>(
                name: "BoothCode",
                table: "Result",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoothCode",
                table: "Result");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDecUpdatedAt",
                table: "Result",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}

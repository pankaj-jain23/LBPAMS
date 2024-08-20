using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class resultgetapiFKEY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDecCreatedAt",
                table: "Result",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResultDecStatus",
                table: "Result",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDecUpdatedAt",
                table: "Result",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultDecCreatedAt",
                table: "Result");

            migrationBuilder.DropColumn(
                name: "ResultDecStatus",
                table: "Result");

            migrationBuilder.DropColumn(
                name: "ResultDecUpdatedAt",
                table: "Result");
        }
    }
}

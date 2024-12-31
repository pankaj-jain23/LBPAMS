using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedSlotinPoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "PollDetails",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLastSlot",
                table: "PollDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "LockTime",
                table: "PollDetails",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "PollDetails",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "PollDetails",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "IsLastSlot",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "LockTime",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "PollDetails");
        }
    }
}

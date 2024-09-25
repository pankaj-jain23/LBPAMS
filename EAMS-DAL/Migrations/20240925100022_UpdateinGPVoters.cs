﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateinGPVoters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WardRangeFrom",
                table: "GPVoter");

            migrationBuilder.RenameColumn(
                name: "WardRangeTo",
                table: "GPVoter",
                newName: "WardRange");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WardRange",
                table: "GPVoter",
                newName: "WardRangeTo");

            migrationBuilder.AddColumn<string>(
                name: "WardRangeFrom",
                table: "GPVoter",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

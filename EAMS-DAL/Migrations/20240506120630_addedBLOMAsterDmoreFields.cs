using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedBLOMAsterDmoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppPin",
                table: "BLOMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "BLOMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "BLOMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OTPAttempts",
                table: "BLOMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpireTime",
                table: "BLOMaster",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPGeneratedTime",
                table: "BLOMaster",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "BLOMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "BLOMaster",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppPin",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "OTP",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "OTPAttempts",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "OTPExpireTime",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "OTPGeneratedTime",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "BLOMaster");
        }
    }
}

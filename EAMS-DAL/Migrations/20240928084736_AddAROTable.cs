using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAROTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StateMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FourthLevelHMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ElectionTypeMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DistrictMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssemblyMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoothMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CandidateType",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsWinner",
                table: "ResultDeclaration",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AROResultMaster",
                columns: table => new
                {
                    AROMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    AROName = table.Column<string>(type: "text", nullable: false),
                    ARODesignation = table.Column<string>(type: "text", nullable: false),
                    AROOfficeName = table.Column<string>(type: "text", nullable: false),
                    AROMobile = table.Column<string>(type: "text", nullable: false),
                    AROCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AROUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsStatus = table.Column<bool>(type: "boolean", nullable: false),
                    OTPGeneratedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTP = table.Column<string>(type: "text", nullable: true),
                    OTPExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTPAttempts = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppPin = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AROResultMaster", x => x.AROMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AROResultMaster");

            migrationBuilder.DropColumn(
                name: "BoothMasterId",
                table: "ResultDeclaration");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "ResultDeclaration");

            migrationBuilder.DropColumn(
                name: "CandidateType",
                table: "ResultDeclaration");

            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "ResultDeclaration");

            migrationBuilder.AlterColumn<int>(
                name: "StateMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FourthLevelHMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ElectionTypeMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DistrictMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AssemblyMasterId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}

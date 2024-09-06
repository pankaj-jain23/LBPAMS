using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedFoMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectorOfficerMaster");

            migrationBuilder.CreateTable(
                name: "FieldOfficerMaster",
                columns: table => new
                {
                    FieldOfficerMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FieldOfficerName = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerDesignation = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerOfficeName = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerMobile = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FieldOfficerUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FieldOfficerStatus = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_FieldOfficerMaster", x => x.FieldOfficerMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldOfficerMaster");

            migrationBuilder.CreateTable(
                name: "SectorOfficerMaster",
                columns: table => new
                {
                    SOMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppPin = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    OTP = table.Column<string>(type: "text", nullable: true),
                    OTPAttempts = table.Column<int>(type: "integer", nullable: false),
                    OTPExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTPGeneratedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SOUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoDesignation = table.Column<string>(type: "text", nullable: false),
                    SoMobile = table.Column<string>(type: "text", nullable: false),
                    SoName = table.Column<string>(type: "text", nullable: false),
                    SoOfficeName = table.Column<string>(type: "text", nullable: false),
                    SoStatus = table.Column<bool>(type: "boolean", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorOfficerMaster", x => x.SOMasterId);
                });
        }
    }
}

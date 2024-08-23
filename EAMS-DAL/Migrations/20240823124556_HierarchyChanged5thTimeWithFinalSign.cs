using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class HierarchyChanged5thTimeWithFinalSign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockZonePanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.DropTable(
                name: "SarpanchWards");

            migrationBuilder.DropTable(
                name: "BlockZonePanchayat");

            migrationBuilder.RenameColumn(
                name: "SoAssemblyCode",
                table: "SectorOfficerMaster",
                newName: "DistrictMasterId");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatMasterId",
                table: "BoothMaster",
                newName: "PSZonePanchayatMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_BoothMaster_BlockZonePanchayatMasterId",
                table: "BoothMaster",
                newName: "IX_BoothMaster_PSZonePanchayatMasterId");

            migrationBuilder.AddColumn<int>(
                name: "AssemblyMasterId",
                table: "SectorOfficerMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FourthLevelHMasterId",
                table: "SectorOfficerMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GPPanchayatWards",
                columns: table => new
                {
                    SarpanchWardsMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SarpanchWardsName = table.Column<string>(type: "text", nullable: false),
                    SarpanchWardsCode = table.Column<int>(type: "integer", nullable: false),
                    SarpanchWardsType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    SarpanchWardsCategory = table.Column<string>(type: "text", nullable: true),
                    SarpanchWardsCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPPanchayatWards", x => x.SarpanchWardsMasterId);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PSZonePanchayat",
                columns: table => new
                {
                    PSZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PSZonePanchayatName = table.Column<string>(type: "text", nullable: false),
                    PSZonePanchayatCode = table.Column<int>(type: "integer", nullable: false),
                    PSZonePanchayatType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatBooths = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatCategory = table.Column<string>(type: "text", nullable: true),
                    BlockZonePanchayatCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockZonePanchayatUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockZonePanchayatDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockZonePanchayatStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSZonePanchayat", x => x.PSZonePanchayatMasterId);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_AssemblyMasterId",
                table: "GPPanchayatWards",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_DistrictMasterId",
                table: "GPPanchayatWards",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_ElectionTypeMasterId",
                table: "GPPanchayatWards",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_FourthLevelHMasterId",
                table: "GPPanchayatWards",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_StateMasterId",
                table: "GPPanchayatWards",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_AssemblyMasterId",
                table: "PSZonePanchayat",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_DistrictMasterId",
                table: "PSZonePanchayat",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_ElectionTypeMasterId",
                table: "PSZonePanchayat",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_FourthLevelHMasterId",
                table: "PSZonePanchayat",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_StateMasterId",
                table: "PSZonePanchayat",
                column: "StateMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_PSZonePanchayat_PSZonePanchayatMasterId",
                table: "BoothMaster",
                column: "PSZonePanchayatMasterId",
                principalTable: "PSZonePanchayat",
                principalColumn: "PSZonePanchayatMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_PSZonePanchayat_PSZonePanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.DropTable(
                name: "GPPanchayatWards");

            migrationBuilder.DropTable(
                name: "PSZonePanchayat");

            migrationBuilder.DropColumn(
                name: "AssemblyMasterId",
                table: "SectorOfficerMaster");

            migrationBuilder.DropColumn(
                name: "FourthLevelHMasterId",
                table: "SectorOfficerMaster");

            migrationBuilder.RenameColumn(
                name: "DistrictMasterId",
                table: "SectorOfficerMaster",
                newName: "SoAssemblyCode");

            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatMasterId",
                table: "BoothMaster",
                newName: "BlockZonePanchayatMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_BoothMaster_PSZonePanchayatMasterId",
                table: "BoothMaster",
                newName: "IX_BoothMaster_BlockZonePanchayatMasterId");

            migrationBuilder.CreateTable(
                name: "BlockZonePanchayat",
                columns: table => new
                {
                    BlockZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatBooths = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatCategory = table.Column<string>(type: "text", nullable: true),
                    BlockZonePanchayatCode = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockZonePanchayatDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockZonePanchayatName = table.Column<string>(type: "text", nullable: false),
                    BlockZonePanchayatStatus = table.Column<bool>(type: "boolean", nullable: false),
                    BlockZonePanchayatType = table.Column<string>(type: "text", nullable: true),
                    BlockZonePanchayatUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockZonePanchayat", x => x.BlockZonePanchayatMasterId);
                    table.ForeignKey(
                        name: "FK_BlockZonePanchayat_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockZonePanchayat_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockZonePanchayat_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockZonePanchayat_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockZonePanchayat_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SarpanchWards",
                columns: table => new
                {
                    SarpanchWardsMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    SarpanchWardsCategory = table.Column<string>(type: "text", nullable: true),
                    SarpanchWardsCode = table.Column<int>(type: "integer", nullable: false),
                    SarpanchWardsCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsName = table.Column<string>(type: "text", nullable: false),
                    SarpanchWardsStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SarpanchWardsType = table.Column<string>(type: "text", nullable: true),
                    SarpanchWardsUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SarpanchWards", x => x.SarpanchWardsMasterId);
                    table.ForeignKey(
                        name: "FK_SarpanchWards_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SarpanchWards_BlockZonePanchayat_BlockZonePanchayatMasterId",
                        column: x => x.BlockZonePanchayatMasterId,
                        principalTable: "BlockZonePanchayat",
                        principalColumn: "BlockZonePanchayatMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SarpanchWards_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SarpanchWards_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SarpanchWards_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SarpanchWards_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockZonePanchayat_AssemblyMasterId",
                table: "BlockZonePanchayat",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockZonePanchayat_DistrictMasterId",
                table: "BlockZonePanchayat",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockZonePanchayat_ElectionTypeMasterId",
                table: "BlockZonePanchayat",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockZonePanchayat_FourthLevelHMasterId",
                table: "BlockZonePanchayat",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockZonePanchayat_StateMasterId",
                table: "BlockZonePanchayat",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_AssemblyMasterId",
                table: "SarpanchWards",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_BlockZonePanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockZonePanchayatMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_DistrictMasterId",
                table: "SarpanchWards",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_ElectionTypeMasterId",
                table: "SarpanchWards",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_FourthLevelHMasterId",
                table: "SarpanchWards",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_StateMasterId",
                table: "SarpanchWards",
                column: "StateMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockZonePanchayatMasterId",
                table: "BoothMaster",
                column: "BlockZonePanchayatMasterId",
                principalTable: "BlockZonePanchayat",
                principalColumn: "BlockZonePanchayatMasterId");
        }
    }
}

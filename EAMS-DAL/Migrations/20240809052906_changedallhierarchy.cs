using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedallhierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_BlockPanchayat_BlockPanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_SarpanchWards_BlockPanchayat_BlockPanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropTable(
                name: "BlockPanchayat");

            migrationBuilder.DropTable(
                name: "PSZone");

            migrationBuilder.DropIndex(
                name: "IX_SarpanchWards_BlockPanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "BlockPanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.RenameColumn(
                name: "PSZoneMasterId",
                table: "BoothMaster",
                newName: "FourthLevelHMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_BoothMaster_PSZoneMasterId",
                table: "BoothMaster",
                newName: "IX_BoothMaster_FourthLevelHMasterId");

            migrationBuilder.AddColumn<int>(
                name: "BlockZonePanchayatMasterId",
                table: "SarpanchWards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FourthLevelHMasterId",
                table: "SarpanchWards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FourthLevelH",
                columns: table => new
                {
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HierarchyName = table.Column<string>(type: "text", nullable: true),
                    HierarchyCode = table.Column<int>(type: "integer", nullable: false),
                    HierarchyType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    HierarchyCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HierarchyUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HierarchyDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HierarchyStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FourthLevelH", x => x.FourthLevelHMasterId);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockZonePanchayat",
                columns: table => new
                {
                    BlockZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlockZonePanchayatName = table.Column<string>(type: "text", nullable: false),
                    BlockZonePanchayatCode = table.Column<int>(type: "integer", nullable: false),
                    BlockZonePanchayatType = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_BlockZonePanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockZonePanchayatMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_FourthLevelHMasterId",
                table: "SarpanchWards",
                column: "FourthLevelHMasterId");

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
                name: "IX_FourthLevelH_AssemblyMasterId",
                table: "FourthLevelH",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_DistrictMasterId",
                table: "FourthLevelH",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_ElectionTypeMasterId",
                table: "FourthLevelH",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_StateMasterId",
                table: "FourthLevelH",
                column: "StateMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockPanchayatMasterId",
                table: "BoothMaster",
                column: "BlockPanchayatMasterId",
                principalTable: "BlockZonePanchayat",
                principalColumn: "BlockZonePanchayatMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_FourthLevelH_FourthLevelHMasterId",
                table: "BoothMaster",
                column: "FourthLevelHMasterId",
                principalTable: "FourthLevelH",
                principalColumn: "FourthLevelHMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SarpanchWards_BlockZonePanchayat_BlockZonePanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockZonePanchayatMasterId",
                principalTable: "BlockZonePanchayat",
                principalColumn: "BlockZonePanchayatMasterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SarpanchWards_FourthLevelH_FourthLevelHMasterId",
                table: "SarpanchWards",
                column: "FourthLevelHMasterId",
                principalTable: "FourthLevelH",
                principalColumn: "FourthLevelHMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockPanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_FourthLevelH_FourthLevelHMasterId",
                table: "BoothMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_SarpanchWards_BlockZonePanchayat_BlockZonePanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropForeignKey(
                name: "FK_SarpanchWards_FourthLevelH_FourthLevelHMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropTable(
                name: "BlockZonePanchayat");

            migrationBuilder.DropTable(
                name: "FourthLevelH");

            migrationBuilder.DropIndex(
                name: "IX_SarpanchWards_BlockZonePanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropIndex(
                name: "IX_SarpanchWards_FourthLevelHMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "FourthLevelHMasterId",
                table: "SarpanchWards");

            migrationBuilder.RenameColumn(
                name: "FourthLevelHMasterId",
                table: "BoothMaster",
                newName: "PSZoneMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_BoothMaster_FourthLevelHMasterId",
                table: "BoothMaster",
                newName: "IX_BoothMaster_PSZoneMasterId");

            migrationBuilder.AddColumn<int>(
                name: "BlockPanchayatMasterId",
                table: "SarpanchWards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlockPanchayat",
                columns: table => new
                {
                    BlockPanchayatMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    BlockPanchayatBooths = table.Column<int>(type: "integer", nullable: false),
                    BlockPanchayatCategory = table.Column<string>(type: "text", nullable: true),
                    BlockPanchayatCode = table.Column<int>(type: "integer", nullable: false),
                    BlockPanchayatCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockPanchayatDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockPanchayatName = table.Column<string>(type: "text", nullable: false),
                    BlockPanchayatStatus = table.Column<bool>(type: "boolean", nullable: false),
                    BlockPanchayatType = table.Column<string>(type: "text", nullable: true),
                    BlockPanchayatUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockPanchayat", x => x.BlockPanchayatMasterId);
                    table.ForeignKey(
                        name: "FK_BlockPanchayat_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockPanchayat_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockPanchayat_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockPanchayat_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PSZone",
                columns: table => new
                {
                    PSZoneMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    PSZoneBooths = table.Column<int>(type: "integer", nullable: false),
                    PSZoneCategory = table.Column<string>(type: "text", nullable: true),
                    PSZoneCode = table.Column<int>(type: "integer", nullable: false),
                    PSZoneCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZoneDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZoneName = table.Column<string>(type: "text", nullable: false),
                    PSZoneStatus = table.Column<bool>(type: "boolean", nullable: false),
                    PSZoneType = table.Column<string>(type: "text", nullable: true),
                    PSZoneUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSZone", x => x.PSZoneMasterId);
                    table.ForeignKey(
                        name: "FK_PSZone_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZone_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZone_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZone_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_BlockPanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockPanchayatMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockPanchayat_AssemblyMasterId",
                table: "BlockPanchayat",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockPanchayat_DistrictMasterId",
                table: "BlockPanchayat",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockPanchayat_ElectionTypeMasterId",
                table: "BlockPanchayat",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockPanchayat_StateMasterId",
                table: "BlockPanchayat",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZone_AssemblyMasterId",
                table: "PSZone",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZone_DistrictMasterId",
                table: "PSZone",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZone_ElectionTypeMasterId",
                table: "PSZone",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZone_StateMasterId",
                table: "PSZone",
                column: "StateMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_BlockPanchayat_BlockPanchayatMasterId",
                table: "BoothMaster",
                column: "BlockPanchayatMasterId",
                principalTable: "BlockPanchayat",
                principalColumn: "BlockPanchayatMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster",
                column: "PSZoneMasterId",
                principalTable: "PSZone",
                principalColumn: "PSZoneMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SarpanchWards_BlockPanchayat_BlockPanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockPanchayatMasterId",
                principalTable: "BlockPanchayat",
                principalColumn: "BlockPanchayatMasterId");
        }
    }
}

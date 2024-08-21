using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedPSSarpanch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "BoothMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PSZoneMasterId",
                table: "BoothMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PSZone",
                columns: table => new
                {
                    PSZoneMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PSZoneName = table.Column<string>(type: "text", nullable: false),
                    PSZoneCode = table.Column<int>(type: "integer", nullable: false),
                    PSZoneType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    PSZoneBooths = table.Column<int>(type: "integer", nullable: false),
                    PSZoneCategory = table.Column<string>(type: "text", nullable: true),
                    PSZoneCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZoneUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZoneDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZoneStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
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
                        name: "FK_PSZone_ParliamentConstituencyMaster_PCMasterId",
                        column: x => x.PCMasterId,
                        principalTable: "ParliamentConstituencyMaster",
                        principalColumn: "PCMasterId");
                    table.ForeignKey(
                        name: "FK_PSZone_StateMaster_StateMasterId",
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
                    SarpanchWardsName = table.Column<string>(type: "text", nullable: false),
                    SarpanchWardsCode = table.Column<int>(type: "integer", nullable: false),
                    SarpanchWardsType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    SarpanchWardsCategory = table.Column<string>(type: "text", nullable: true),
                    SarpanchWardsCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SarpanchWardsStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
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
                        name: "FK_SarpanchWards_BoothMaster_BoothMasterId",
                        column: x => x.BoothMasterId,
                        principalTable: "BoothMaster",
                        principalColumn: "BoothMasterId",
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
                        name: "FK_SarpanchWards_ParliamentConstituencyMaster_PCMasterId",
                        column: x => x.PCMasterId,
                        principalTable: "ParliamentConstituencyMaster",
                        principalColumn: "PCMasterId");
                    table.ForeignKey(
                        name: "FK_SarpanchWards_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_PSZoneMasterId",
                table: "BoothMaster",
                column: "PSZoneMasterId");

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
                name: "IX_PSZone_PCMasterId",
                table: "PSZone",
                column: "PCMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZone_StateMasterId",
                table: "PSZone",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_AssemblyMasterId",
                table: "SarpanchWards",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_BoothMasterId",
                table: "SarpanchWards",
                column: "BoothMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_DistrictMasterId",
                table: "SarpanchWards",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_ElectionTypeMasterId",
                table: "SarpanchWards",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_PCMasterId",
                table: "SarpanchWards",
                column: "PCMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_StateMasterId",
                table: "SarpanchWards",
                column: "StateMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster",
                column: "PSZoneMasterId",
                principalTable: "PSZone",
                principalColumn: "PSZoneMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster");

            migrationBuilder.DropTable(
                name: "PSZone");

            migrationBuilder.DropTable(
                name: "SarpanchWards");

            migrationBuilder.DropIndex(
                name: "IX_BoothMaster_PSZoneMasterId",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "PSZoneMasterId",
                table: "BoothMaster");
        }
    }
}

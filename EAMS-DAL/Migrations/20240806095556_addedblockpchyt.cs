using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedblockpchyt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockPanchayatMasterId",
                table: "BoothMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlockPanchayat",
                columns: table => new
                {
                    BlockPanchayatMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlockPanchayatName = table.Column<string>(type: "text", nullable: false),
                    BlockPanchayatCode = table.Column<int>(type: "integer", nullable: false),
                    BlockPanchayatType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BlockPanchayatBooths = table.Column<int>(type: "integer", nullable: false),
                    BlockPanchayatCategory = table.Column<string>(type: "text", nullable: true),
                    BlockPanchayatCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockPanchayatUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockPanchayatDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockPanchayatStatus = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_BlockPanchayatMasterId",
                table: "BoothMaster",
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

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_BlockPanchayat_BlockPanchayatMasterId",
                table: "BoothMaster",
                column: "BlockPanchayatMasterId",
                principalTable: "BlockPanchayat",
                principalColumn: "BlockPanchayatMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_BlockPanchayat_BlockPanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.DropTable(
                name: "BlockPanchayat");

            migrationBuilder.DropIndex(
                name: "IX_BoothMaster_BlockPanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "BlockPanchayatMasterId",
                table: "BoothMaster");
        }
    }
}

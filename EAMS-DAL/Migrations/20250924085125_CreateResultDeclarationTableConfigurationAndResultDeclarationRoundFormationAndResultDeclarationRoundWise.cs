using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreateResultDeclarationTableConfigurationAndResultDeclarationRoundFormationAndResultDeclarationRoundWise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultDeclarationTableConfigurations",
                columns: table => new
                {
                    RDTableConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    NoOfBooth = table.Column<int>(type: "integer", nullable: true),
                    NoOfRound = table.Column<int>(type: "integer", nullable: true),
                    NoOfTable = table.Column<int>(type: "integer", nullable: true),
                    IsFinalized = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDeclarationTableConfigurations", x => x.RDTableConfigId);
                });

            migrationBuilder.CreateTable(
                name: "ResultDeclarationRoundFormations",
                columns: table => new
                {
                    RDRoundFormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RDTableConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundName = table.Column<string>(type: "text", nullable: true),
                    RoundNumber = table.Column<int>(type: "integer", nullable: true),
                    IsFinalized = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDeclarationRoundFormations", x => x.RDRoundFormationId);
                    table.ForeignKey(
                        name: "FK_ResultDeclarationRoundFormations_ResultDeclarationTableConf~",
                        column: x => x.RDTableConfigId,
                        principalTable: "ResultDeclarationTableConfigurations",
                        principalColumn: "RDTableConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultDeclarationRoundWises",
                columns: table => new
                {
                    RDRoundWiseId = table.Column<Guid>(type: "uuid", nullable: false),
                    RDTableConfigId = table.Column<Guid>(type: "uuid", nullable: true),
                    RDRoundFormationId = table.Column<Guid>(type: "uuid", nullable: true),
                    KycMasterId = table.Column<int>(type: "integer", nullable: false),
                    TotalVotes = table.Column<int>(type: "integer", nullable: true),
                    TotalCommulativeVotes = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDeclarationRoundWises", x => x.RDRoundWiseId);
                    table.ForeignKey(
                        name: "FK_ResultDeclarationRoundWises_ResultDeclarationRoundFormation~",
                        column: x => x.RDRoundFormationId,
                        principalTable: "ResultDeclarationRoundFormations",
                        principalColumn: "RDRoundFormationId");
                    table.ForeignKey(
                        name: "FK_ResultDeclarationRoundWises_ResultDeclarationTableConfigura~",
                        column: x => x.RDTableConfigId,
                        principalTable: "ResultDeclarationTableConfigurations",
                        principalColumn: "RDTableConfigId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultDeclarationRoundFormations_RDTableConfigId",
                table: "ResultDeclarationRoundFormations",
                column: "RDTableConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDeclarationRoundWises_RDRoundFormationId",
                table: "ResultDeclarationRoundWises",
                column: "RDRoundFormationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDeclarationRoundWises_RDTableConfigId",
                table: "ResultDeclarationRoundWises",
                column: "RDTableConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultDeclarationRoundWises");

            migrationBuilder.DropTable(
                name: "ResultDeclarationRoundFormations");

            migrationBuilder.DropTable(
                name: "ResultDeclarationTableConfigurations");
        }
    }
}

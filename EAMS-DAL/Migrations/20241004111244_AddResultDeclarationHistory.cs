using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddResultDeclarationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRecounting",
                table: "ResultDeclaration",
                newName: "IsReCounting");

            migrationBuilder.AddColumn<bool>(
                name: "IsDraw",
                table: "ResultDeclaration",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDrawLottery",
                table: "ResultDeclaration",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ResultDeclarationHistory",
                columns: table => new
                {
                    ResultDeclarationHistoryMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultDeclarationMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: true),
                    GPPanchayatWardsMasterId = table.Column<int>(type: "integer", nullable: true),
                    KycMasterId = table.Column<int>(type: "integer", nullable: false),
                    VoteMargin = table.Column<string>(type: "text", nullable: true),
                    IsWinner = table.Column<bool>(type: "boolean", nullable: false),
                    IsResultDeclared = table.Column<bool>(type: "boolean", nullable: false),
                    IsReCounting = table.Column<bool>(type: "boolean", nullable: false),
                    IsDraw = table.Column<bool>(type: "boolean", nullable: false),
                    IsDrawLottery = table.Column<bool>(type: "boolean", nullable: false),
                    ResultDecCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDeclarationHistory", x => x.ResultDeclarationHistoryMasterId);
                    table.ForeignKey(
                        name: "FK_ResultDeclarationHistory_ResultDeclaration_ResultDeclaratio~",
                        column: x => x.ResultDeclarationMasterId,
                        principalTable: "ResultDeclaration",
                        principalColumn: "ResultDeclarationMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultDeclarationHistory_ResultDeclarationMasterId",
                table: "ResultDeclarationHistory",
                column: "ResultDeclarationMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultDeclarationHistory");

            migrationBuilder.DropColumn(
                name: "IsDraw",
                table: "ResultDeclaration");

            migrationBuilder.DropColumn(
                name: "IsDrawLottery",
                table: "ResultDeclaration");

            migrationBuilder.RenameColumn(
                name: "IsReCounting",
                table: "ResultDeclaration",
                newName: "IsRecounting");
        }
    }
}

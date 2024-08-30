using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddResultDeclarationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultDeclaration",
                columns: table => new
                {
                    ResultDeclarationMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    GPPanchayatWardsMasterId = table.Column<int>(type: "integer", nullable: true),
                    CandidateName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    VoteMargin = table.Column<string>(type: "text", nullable: true),
                    ResultDecCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDeclaration", x => x.ResultDeclarationMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultDeclaration");
        }
    }
}

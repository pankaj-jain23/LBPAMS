using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedGpVoters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "GPVoter",
                columns: table => new
                {
                    GPVoterMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    GPVoterPdfPath = table.Column<string>(type: "text", nullable: true),
                    GPVoterCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPVoterUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPVoterDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPVoterStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPVoter", x => x.GPVoterMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GPVoter");

          
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedPPR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PPR",
                columns: table => new
                {
                    PPRMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    PCCode = table.Column<string>(type: "text", nullable: true),
                    DistrictCode = table.Column<string>(type: "text", nullable: true),
                    DistrictName = table.Column<string>(type: "text", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyCode = table.Column<string>(type: "text", nullable: false),
                    AssemblyName = table.Column<string>(type: "text", nullable: false),
                    PP_R1 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PP_R2 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PP_R3 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MO_R1 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MO_R2 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CO_R1 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CO_R2 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CO_R3 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPR", x => x.PPRMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPR");
        }
    }
}

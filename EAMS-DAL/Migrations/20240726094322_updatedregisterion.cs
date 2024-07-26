using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatedregisterion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPSZone",
                columns: table => new
                {
                    UserPSZoneId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    UserAssemblyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPSZone", x => x.UserPSZoneId);
                    table.ForeignKey(
                        name: "FK_UserPSZone_UserAssembly_UserAssemblyId",
                        column: x => x.UserAssemblyId,
                        principalTable: "UserAssembly",
                        principalColumn: "UserAssemblyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPSZone_UserAssemblyId",
                table: "UserPSZone",
                column: "UserAssemblyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPSZone");
        }
    }
}

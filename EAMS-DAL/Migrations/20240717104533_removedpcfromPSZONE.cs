using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class removedpcfromPSZONE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PSZone_ParliamentConstituencyMaster_PCMasterId",
                table: "PSZone");

            migrationBuilder.DropIndex(
                name: "IX_PSZone_PCMasterId",
                table: "PSZone");

            migrationBuilder.DropColumn(
                name: "PCMasterId",
                table: "PSZone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PCMasterId",
                table: "PSZone",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PSZone_PCMasterId",
                table: "PSZone",
                column: "PCMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PSZone_ParliamentConstituencyMaster_PCMasterId",
                table: "PSZone",
                column: "PCMasterId",
                principalTable: "ParliamentConstituencyMaster",
                principalColumn: "PCMasterId");
        }
    }
}

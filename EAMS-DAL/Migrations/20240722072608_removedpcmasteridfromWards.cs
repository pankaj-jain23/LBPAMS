using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class removedpcmasteridfromWards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SarpanchWards_ParliamentConstituencyMaster_PCMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropIndex(
                name: "IX_SarpanchWards_PCMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "PCMasterId",
                table: "SarpanchWards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PCMasterId",
                table: "SarpanchWards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_PCMasterId",
                table: "SarpanchWards",
                column: "PCMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SarpanchWards_ParliamentConstituencyMaster_PCMasterId",
                table: "SarpanchWards",
                column: "PCMasterId",
                principalTable: "ParliamentConstituencyMaster",
                principalColumn: "PCMasterId");
        }
    }
}

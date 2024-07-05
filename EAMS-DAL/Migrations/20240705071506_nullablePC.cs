using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class nullablePC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssemblyMaster_ParliamentConstituencyMaster_PCMasterId",
                table: "AssemblyMaster");

            migrationBuilder.AlterColumn<int>(
                name: "PCMasterId",
                table: "BLOMaster",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "PCMasterId",
                table: "AssemblyMaster",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_AssemblyMaster_ParliamentConstituencyMaster_PCMasterId",
                table: "AssemblyMaster",
                column: "PCMasterId",
                principalTable: "ParliamentConstituencyMaster",
                principalColumn: "PCMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssemblyMaster_ParliamentConstituencyMaster_PCMasterId",
                table: "AssemblyMaster");

            migrationBuilder.AlterColumn<int>(
                name: "PCMasterId",
                table: "BLOMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PCMasterId",
                table: "AssemblyMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssemblyMaster_ParliamentConstituencyMaster_PCMasterId",
                table: "AssemblyMaster",
                column: "PCMasterId",
                principalTable: "ParliamentConstituencyMaster",
                principalColumn: "PCMasterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

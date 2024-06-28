using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedelectiontypeallmaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "AssemblyMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyMaster_ElectionTypeMasterId",
                table: "AssemblyMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssemblyMaster_ElectionTypeMaster_ElectionTypeMasterId",
                table: "AssemblyMaster",
                column: "ElectionTypeMasterId",
                principalTable: "ElectionTypeMaster",
                principalColumn: "ElectionTypeMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssemblyMaster_ElectionTypeMaster_ElectionTypeMasterId",
                table: "AssemblyMaster");

            migrationBuilder.DropIndex(
                name: "IX_AssemblyMaster_ElectionTypeMasterId",
                table: "AssemblyMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "AssemblyMaster");
        }
    }
}

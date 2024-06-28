using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedelectiontypeallmaster2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "AssemblyMaster");

            migrationBuilder.RenameColumn(
                name: "ElectionTypeId",
                table: "BoothMaster",
                newName: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_ElectionTypeMasterId",
                table: "BoothMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_ElectionTypeMaster_ElectionTypeMasterId",
                table: "BoothMaster",
                column: "ElectionTypeMasterId",
                principalTable: "ElectionTypeMaster",
                principalColumn: "ElectionTypeMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_ElectionTypeMaster_ElectionTypeMasterId",
                table: "BoothMaster");

            migrationBuilder.DropIndex(
                name: "IX_BoothMaster_ElectionTypeMasterId",
                table: "BoothMaster");

            migrationBuilder.RenameColumn(
                name: "ElectionTypeMasterId",
                table: "BoothMaster",
                newName: "ElectionTypeId");

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "AssemblyMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

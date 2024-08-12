using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedBoothMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockPanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.RenameColumn(
                name: "BlockPanchayatMasterId",
                table: "BoothMaster",
                newName: "BlockZonePanchayatMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_BoothMaster_BlockPanchayatMasterId",
                table: "BoothMaster",
                newName: "IX_BoothMaster_BlockZonePanchayatMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockZonePanchayatMasterId",
                table: "BoothMaster",
                column: "BlockZonePanchayatMasterId",
                principalTable: "BlockZonePanchayat",
                principalColumn: "BlockZonePanchayatMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockZonePanchayatMasterId",
                table: "BoothMaster");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatMasterId",
                table: "BoothMaster",
                newName: "BlockPanchayatMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_BoothMaster_BlockZonePanchayatMasterId",
                table: "BoothMaster",
                newName: "IX_BoothMaster_BlockPanchayatMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_BlockZonePanchayat_BlockPanchayatMasterId",
                table: "BoothMaster",
                column: "BlockPanchayatMasterId",
                principalTable: "BlockZonePanchayat",
                principalColumn: "BlockZonePanchayatMasterId");
        }
    }
}

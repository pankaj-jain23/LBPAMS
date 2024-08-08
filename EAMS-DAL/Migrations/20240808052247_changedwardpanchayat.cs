using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedwardpanchayat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SarpanchWards_BoothMaster_BoothMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropIndex(
                name: "IX_SarpanchWards_BoothMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "BoothMasterId",
                table: "SarpanchWards");

            migrationBuilder.AddColumn<int>(
                name: "BlockPanchayatMasterId",
                table: "SarpanchWards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_BlockPanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockPanchayatMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SarpanchWards_BlockPanchayat_BlockPanchayatMasterId",
                table: "SarpanchWards",
                column: "BlockPanchayatMasterId",
                principalTable: "BlockPanchayat",
                principalColumn: "BlockPanchayatMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SarpanchWards_BlockPanchayat_BlockPanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropIndex(
                name: "IX_SarpanchWards_BlockPanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "BlockPanchayatMasterId",
                table: "SarpanchWards");

            migrationBuilder.AddColumn<int>(
                name: "BoothMasterId",
                table: "SarpanchWards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SarpanchWards_BoothMasterId",
                table: "SarpanchWards",
                column: "BoothMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SarpanchWards_BoothMaster_BoothMasterId",
                table: "SarpanchWards",
                column: "BoothMasterId",
                principalTable: "BoothMaster",
                principalColumn: "BoothMasterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatedElectionTypeMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ElectionTypeMasterId",
                table: "AspNetUsers",
                column: "ElectionTypeMasterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ElectionTypeMaster_ElectionTypeMasterId",
                table: "AspNetUsers",
                column: "ElectionTypeMasterId",
                principalTable: "ElectionTypeMaster",
                principalColumn: "ElectionTypeMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ElectionTypeMaster_ElectionTypeMasterId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ElectionTypeMasterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "AspNetUsers");
        }
    }
}

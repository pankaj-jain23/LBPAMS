using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedElectionTypeInEventMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "EventMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventMaster_ElectionTypeMasterId",
                table: "EventMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventMaster_ElectionTypeMaster_ElectionTypeMasterId",
                table: "EventMaster",
                column: "ElectionTypeMasterId",
                principalTable: "ElectionTypeMaster",
                principalColumn: "ElectionTypeMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventMaster_ElectionTypeMaster_ElectionTypeMasterId",
                table: "EventMaster");

            migrationBuilder.DropIndex(
                name: "IX_EventMaster_ElectionTypeMasterId",
                table: "EventMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "EventMaster");
        }
    }
}

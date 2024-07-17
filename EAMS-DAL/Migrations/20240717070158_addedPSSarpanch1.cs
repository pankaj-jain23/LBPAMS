using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedPSSarpanch1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster");

            migrationBuilder.AlterColumn<int>(
                name: "PSZoneMasterId",
                table: "BoothMaster",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster",
                column: "PSZoneMasterId",
                principalTable: "PSZone",
                principalColumn: "PSZoneMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster");

            migrationBuilder.AlterColumn<int>(
                name: "PSZoneMasterId",
                table: "BoothMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BoothMaster_PSZone_PSZoneMasterId",
                table: "BoothMaster",
                column: "PSZoneMasterId",
                principalTable: "PSZone",
                principalColumn: "PSZoneMasterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

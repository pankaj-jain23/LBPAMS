using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedBLOBOothMasterFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoothCode",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "BoothMasterId",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "PSBuildingName",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "PartNameEn",
                table: "BLOMaster");

            migrationBuilder.AddColumn<bool>(
                name: "BLOStatus",
                table: "BLOBoothMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PSBuildingName",
                table: "BLOBoothMaster",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BLOStatus",
                table: "BLOBoothMaster");

            migrationBuilder.DropColumn(
                name: "PSBuildingName",
                table: "BLOBoothMaster");

            migrationBuilder.AddColumn<string>(
                name: "BoothCode",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BoothMasterId",
                table: "BLOMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PSBuildingName",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartNameEn",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

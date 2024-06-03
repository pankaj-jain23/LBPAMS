using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class freezeandStateDistrictNameCodeinPS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "PollingStationMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "PollingStationMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Freezed",
                table: "PollingStationMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "PollingStationMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateName",
                table: "PollingStationMaster",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "PollingStationMaster");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "PollingStationMaster");

            migrationBuilder.DropColumn(
                name: "Freezed",
                table: "PollingStationMaster");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "PollingStationMaster");

            migrationBuilder.DropColumn(
                name: "StateName",
                table: "PollingStationMaster");
        }
    }
}

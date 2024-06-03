using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedelectiontpyeinMasters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentStateMasterId",
                table: "StateMaster");

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "ParliamentConstituencyMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "DistrictMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "BoothMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "AssemblyMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "ParliamentConstituencyMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "DistrictMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "AssemblyMaster");

            migrationBuilder.AddColumn<int>(
                name: "ParentStateMasterId",
                table: "StateMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

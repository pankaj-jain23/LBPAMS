using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedZoneSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "SarpanchWards");

            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "PSZone");

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "SarpanchWards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "PSZone",
                type: "text",
                nullable: true);
        }
    }
}

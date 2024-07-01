using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class removedstateelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "StateMaster");

            migrationBuilder.DropColumn(
                name: "ElectionTypeId",
                table: "DistrictMaster");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "StateMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeId",
                table: "DistrictMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCCAndIsNNInFouthLevelAndGPWardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCC",
                table: "GPPanchayatWards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNN",
                table: "GPPanchayatWards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCC",
                table: "FourthLevelH",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNN",
                table: "FourthLevelH",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCC",
                table: "GPPanchayatWards");

            migrationBuilder.DropColumn(
                name: "IsNN",
                table: "GPPanchayatWards");

            migrationBuilder.DropColumn(
                name: "IsCC",
                table: "FourthLevelH");

            migrationBuilder.DropColumn(
                name: "IsNN",
                table: "FourthLevelH");
        }
    }
}

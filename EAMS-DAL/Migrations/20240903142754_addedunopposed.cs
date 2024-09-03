using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedunopposed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatMasterId",
                table: "UnOpposed");

            migrationBuilder.DropColumn(
                name: "SarpanchWardsMasterId",
                table: "UnOpposed");

            migrationBuilder.AlterColumn<int>(
                name: "StateMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NominationPdfPath",
                table: "UnOpposed",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FourthLevelHMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GPPanchayatWardsMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PSZonePanchayatMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GPPanchayatWardsMasterId",
                table: "UnOpposed");

            migrationBuilder.DropColumn(
                name: "PSZonePanchayatMasterId",
                table: "UnOpposed");

            migrationBuilder.AlterColumn<int>(
                name: "StateMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "NominationPdfPath",
                table: "UnOpposed",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "FourthLevelHMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BlockZonePanchayatMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SarpanchWardsMasterId",
                table: "UnOpposed",
                type: "integer",
                nullable: true);
        }
    }
}

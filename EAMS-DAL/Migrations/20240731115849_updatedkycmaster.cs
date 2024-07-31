using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatedkycmaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "ElectionType",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "MCorporationMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "MCouncilMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "NPMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "PSMasterId",
                table: "Kyc");

            migrationBuilder.RenameColumn(
                name: "PSZoneMasterId",
                table: "Kyc",
                newName: "PsZoneMasterId");

            migrationBuilder.RenameColumn(
                name: "ZPMasterId",
                table: "Kyc",
                newName: "ElectionTypeMasterId");

            migrationBuilder.RenameColumn(
                name: "WardMasterId",
                table: "Kyc",
                newName: "BoothMasterId");

            migrationBuilder.AlterColumn<int>(
                name: "SarpanchWardsMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "PsZoneMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PsZoneMasterId",
                table: "Kyc",
                newName: "PSZoneMasterId");

            migrationBuilder.RenameColumn(
                name: "ElectionTypeMasterId",
                table: "Kyc",
                newName: "ZPMasterId");

            migrationBuilder.RenameColumn(
                name: "BoothMasterId",
                table: "Kyc",
                newName: "WardMasterId");

            migrationBuilder.AlterColumn<int>(
                name: "SarpanchWardsMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PSZoneMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlockMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ElectionType",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MCorporationMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MCouncilMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NPMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PSMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);
        }
    }
}

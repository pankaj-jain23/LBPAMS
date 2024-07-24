using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatekyc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NagaraPanchayatMasterId",
                table: "Kyc",
                newName: "WardMasterId");

            migrationBuilder.AddColumn<int>(
                name: "NPMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PSZoneMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SarpanchWardsMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NPMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "PSZoneMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "SarpanchWardsMasterId",
                table: "Kyc");

            migrationBuilder.RenameColumn(
                name: "WardMasterId",
                table: "Kyc",
                newName: "NagaraPanchayatMasterId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMaleFemaleTransgenderAndTotalVotersInFourthLevelHAndGPPanchayatWard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Female",
                table: "GPPanchayatWards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                table: "GPPanchayatWards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalVoters",
                table: "GPPanchayatWards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transgender",
                table: "GPPanchayatWards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Female",
                table: "FourthLevelH",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                table: "FourthLevelH",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalVoters",
                table: "FourthLevelH",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transgender",
                table: "FourthLevelH",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Female",
                table: "GPPanchayatWards");

            migrationBuilder.DropColumn(
                name: "Male",
                table: "GPPanchayatWards");

            migrationBuilder.DropColumn(
                name: "TotalVoters",
                table: "GPPanchayatWards");

            migrationBuilder.DropColumn(
                name: "Transgender",
                table: "GPPanchayatWards");

            migrationBuilder.DropColumn(
                name: "Female",
                table: "FourthLevelH");

            migrationBuilder.DropColumn(
                name: "Male",
                table: "FourthLevelH");

            migrationBuilder.DropColumn(
                name: "TotalVoters",
                table: "FourthLevelH");

            migrationBuilder.DropColumn(
                name: "Transgender",
                table: "FourthLevelH");
        }
    }
}

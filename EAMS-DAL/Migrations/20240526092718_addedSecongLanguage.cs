using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedSecongLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "StateMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "ParliamentConstituencyMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "DistrictMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "BoothMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLanguage",
                table: "AssemblyMaster",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "StateMaster");

            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "ParliamentConstituencyMaster");

            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "DistrictMaster");

            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "SecondLanguage",
                table: "AssemblyMaster");
        }
    }
}

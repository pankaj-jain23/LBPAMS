using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class MaleFemaleTransgenderinElectionInfoMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Female",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transgender",
                table: "ElectionInfoMaster",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Female",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "Male",
                table: "ElectionInfoMaster");

            migrationBuilder.DropColumn(
                name: "Transgender",
                table: "ElectionInfoMaster");
        }
    }
}

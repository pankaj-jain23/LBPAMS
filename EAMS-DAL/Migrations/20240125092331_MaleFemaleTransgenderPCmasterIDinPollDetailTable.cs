using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class MaleFemaleTransgenderPCmasterIDinPollDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Female",
                table: "PollDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                table: "PollDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PCMasterId",
                table: "PollDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transgender",
                table: "PollDetails",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Female",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "Male",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "PCMasterId",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "Transgender",
                table: "PollDetails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class removedrandomizationfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssemblyMasterId",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "PCMasterId",
                table: "PPR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssemblyMasterId",
                table: "PPR",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PCMasterId",
                table: "PPR",
                type: "integer",
                nullable: true);
        }
    }
}

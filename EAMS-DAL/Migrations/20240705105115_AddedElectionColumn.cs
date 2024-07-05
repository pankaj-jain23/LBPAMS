using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedElectionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hierarchy1",
                table: "ElectionTypeMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hierarchy2",
                table: "ElectionTypeMaster",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hierarchy1",
                table: "ElectionTypeMaster");

            migrationBuilder.DropColumn(
                name: "Hierarchy2",
                table: "ElectionTypeMaster");
        }
    }
}

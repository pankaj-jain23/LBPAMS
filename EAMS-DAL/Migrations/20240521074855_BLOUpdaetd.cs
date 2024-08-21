using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class BLOUpdaetd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BLOCreatedAt",
                table: "BLOMaster",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BLOUpdatedAt",
                table: "BLOMaster",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BLOCreatedAt",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "BLOUpdatedAt",
                table: "BLOMaster");
        }
    }
}

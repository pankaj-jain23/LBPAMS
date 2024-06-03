using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedBLOMAsterDFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "BLOMaster",
                newName: "BLOMobile");

            migrationBuilder.AddColumn<bool>(
                name: "BLOStatus",
                table: "BLOMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BLOStatus",
                table: "BLOMaster");

            migrationBuilder.RenameColumn(
                name: "BLOMobile",
                table: "BLOMaster",
                newName: "Mobile");
        }
    }
}

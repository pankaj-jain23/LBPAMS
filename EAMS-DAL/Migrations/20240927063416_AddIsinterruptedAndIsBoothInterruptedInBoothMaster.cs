using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsinterruptedAndIsBoothInterruptedInBoothMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBoothInterrupted",
                table: "BoothMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVTInterrupted",
                table: "BoothMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBoothInterrupted",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "IsVTInterrupted",
                table: "BoothMaster");
        }
    }
}

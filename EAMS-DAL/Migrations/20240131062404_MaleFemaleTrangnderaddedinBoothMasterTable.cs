using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class MaleFemaleTrangnderaddedinBoothMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Female",
                table: "BoothMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                table: "BoothMaster",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transgender",
                table: "BoothMaster",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Female",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "Male",
                table: "BoothMaster");

            migrationBuilder.DropColumn(
                name: "Transgender",
                table: "BoothMaster");
        }
    }
}

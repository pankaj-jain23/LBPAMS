using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations.SMS
{
    /// <inheritdoc />
    public partial class AddedSMSConfiguration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictMasterId",
                table: "SMSNumbers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateMasterId",
                table: "SMSNumbers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "DistrictMasterId",
                table: "SMSConfiguration",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictMasterId",
                table: "SMSNumbers");

            migrationBuilder.DropColumn(
                name: "StateMasterId",
                table: "SMSNumbers");

            migrationBuilder.AlterColumn<int>(
                name: "DistrictMasterId",
                table: "SMSConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}

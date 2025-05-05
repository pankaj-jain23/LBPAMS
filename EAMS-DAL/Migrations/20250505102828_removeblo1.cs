using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeblo1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
             name: "PSZonePanchayatBooths",
             table: "PSZonePanchayat",
             type: "integer",
             nullable: false,
             defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PSZonePanchayatCategory",
                table: "PSZonePanchayat",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PSZonePanchayatCreatedAt",
                table: "PSZonePanchayat",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PSZonePanchayatDeletedAt",
                table: "PSZonePanchayat",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PSZonePanchayatStatus",
                table: "PSZonePanchayat",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PSZonePanchayatUpdatedAt",
                table: "PSZonePanchayat",
                type: "timestamp with time zone",
                nullable: true);
            migrationBuilder.DropColumn(
            name: "BlockZonePanchayatBooths",
            table: "PSZonePanchayat");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatCategory",
                table: "PSZonePanchayat");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatCreatedAt",
                table: "PSZonePanchayat");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatDeletedAt",
                table: "PSZonePanchayat");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatStatus",
                table: "PSZonePanchayat");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatUpdatedAt",
                table: "PSZonePanchayat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

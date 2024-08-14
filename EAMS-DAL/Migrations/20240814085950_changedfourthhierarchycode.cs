using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedfourthhierarchycode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoothMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "PsZoneMasterId",
                table: "Kyc");

            migrationBuilder.AlterColumn<int>(
                name: "SarpanchWardsMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlockZonePanchayatMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FourthLevelHMasterId",
                table: "Kyc",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UnOpposed",
                columns: table => new
                {
                    UnOpposedMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    BlockZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: true),
                    SarpanchWardsMasterId = table.Column<int>(type: "integer", nullable: true),
                    CandidateName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    NominationPdfPath = table.Column<string>(type: "text", nullable: true),
                    Option1 = table.Column<string>(type: "text", nullable: true),
                    Option2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnOpposed", x => x.UnOpposedMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnOpposed");

            migrationBuilder.DropColumn(
                name: "BlockZonePanchayatMasterId",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "FourthLevelHMasterId",
                table: "Kyc");

            migrationBuilder.AlterColumn<int>(
                name: "SarpanchWardsMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BoothMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PsZoneMasterId",
                table: "Kyc",
                type: "integer",
                nullable: true);
        }
    }
}

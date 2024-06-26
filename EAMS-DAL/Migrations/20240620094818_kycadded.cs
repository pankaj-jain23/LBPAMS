using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class kycadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kyc",
                columns: table => new
                {
                    KycMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionType = table.Column<int>(type: "integer", nullable: true),
                    BlockMasterId = table.Column<int>(type: "integer", nullable: true),
                    ZPMasterId = table.Column<int>(type: "integer", nullable: true),
                    PSMasterId = table.Column<int>(type: "integer", nullable: true),
                    MCorporationMasterId = table.Column<int>(type: "integer", nullable: true),
                    MCouncilMasterId = table.Column<int>(type: "integer", nullable: true),
                    NagaraPanchayatMasterId = table.Column<int>(type: "integer", nullable: true),
                    Option1 = table.Column<string>(type: "text", nullable: true),
                    Option2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kyc", x => x.KycMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kyc");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedBLOMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BLOMaster",
                columns: table => new
                {
                    BLOMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateCode = table.Column<string>(type: "text", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictCode = table.Column<string>(type: "text", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCCode = table.Column<string>(type: "text", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyCode = table.Column<string>(type: "text", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothCode = table.Column<string>(type: "text", nullable: false),
                    PartNameEn = table.Column<string>(type: "text", nullable: false),
                    PSBuildingName = table.Column<string>(type: "text", nullable: false),
                    Mobile = table.Column<string>(type: "text", nullable: false),
                    BLOName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOMaster", x => x.BLOMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BLOMaster");
        }
    }
}

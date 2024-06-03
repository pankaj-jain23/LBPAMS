using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class removedBLOmaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BLOBoothMaster");

            migrationBuilder.DropColumn(
                name: "AssemblyCode",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "PCCode",
                table: "BLOMaster");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "BLOMaster");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToBLO",
                table: "BoothMaster",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToBLO",
                table: "BoothMaster");

            migrationBuilder.AddColumn<string>(
                name: "AssemblyCode",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PCCode",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "BLOMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BLOBoothMaster",
                columns: table => new
                {
                    BLOBoothMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BLOMasterId = table.Column<int>(type: "integer", nullable: false),
                    BLOBoothCode = table.Column<string>(type: "text", nullable: false),
                    BLOBoothName = table.Column<string>(type: "text", nullable: false),
                    BLOStatus = table.Column<bool>(type: "boolean", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    PSBuildingName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOBoothMaster", x => x.BLOBoothMasterId);
                    table.ForeignKey(
                        name: "FK_BLOBoothMaster_BLOMaster_BLOMasterId",
                        column: x => x.BLOMasterId,
                        principalTable: "BLOMaster",
                        principalColumn: "BLOMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BLOBoothMaster_BLOMasterId",
                table: "BLOBoothMaster",
                column: "BLOMasterId");
        }
    }
}

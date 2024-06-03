using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedBLOBOothMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SOMobileNumber",
                table: "QIS",
                newName: "BLOMobileNumber");

            migrationBuilder.RenameColumn(
                name: "SOId",
                table: "QIS",
                newName: "BLOMasterId");

            migrationBuilder.CreateTable(
                name: "BLOBoothMaster",
                columns: table => new
                {
                    BLOBoothMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BLOBoothName = table.Column<string>(type: "text", nullable: false),
                    BLOBoothCode = table.Column<string>(type: "text", nullable: false),
                    BLOMasterId = table.Column<int>(type: "integer", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BLOBoothMaster");

            migrationBuilder.RenameColumn(
                name: "BLOMobileNumber",
                table: "QIS",
                newName: "SOMobileNumber");

            migrationBuilder.RenameColumn(
                name: "BLOMasterId",
                table: "QIS",
                newName: "SOId");
        }
    }
}

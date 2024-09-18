using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedElectionABBR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventABBR",
                table: "EventMaster",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateMasterId",
                table: "EventMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventAbbr",
                columns: table => new
                {
                    EventAbbrId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventAbbrName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAbbr", x => x.EventAbbrId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventMaster_StateMasterId",
                table: "EventMaster",
                column: "StateMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventMaster_StateMaster_StateMasterId",
                table: "EventMaster",
                column: "StateMasterId",
                principalTable: "StateMaster",
                principalColumn: "StateMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventMaster_StateMaster_StateMasterId",
                table: "EventMaster");

            migrationBuilder.DropTable(
                name: "EventAbbr");

            migrationBuilder.DropIndex(
                name: "IX_EventMaster_StateMasterId",
                table: "EventMaster");

            migrationBuilder.DropColumn(
                name: "EventABBR",
                table: "EventMaster");

            migrationBuilder.DropColumn(
                name: "StateMasterId",
                table: "EventMaster");
        }
    }
}

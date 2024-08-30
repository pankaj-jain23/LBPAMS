using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedAuthenticationModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPSZone");

            migrationBuilder.DropTable(
                name: "UserAssembly");

            migrationBuilder.DropTable(
                name: "UserDistrict");

            migrationBuilder.DropTable(
                name: "UserPCConstituency");

            migrationBuilder.DropTable(
                name: "UserState");

            migrationBuilder.AddColumn<int>(
                name: "AssemblyMasterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictMasterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FourthLevelHMasterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateMasterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssemblyMasterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DistrictMasterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FourthLevelHMasterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StateMasterId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserState",
                columns: table => new
                {
                    UserStateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<string>(type: "text", nullable: true),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserState", x => x.UserStateId);
                    table.ForeignKey(
                        name: "FK_UserState_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserDistrict",
                columns: table => new
                {
                    UserDistrictId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserStateId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDistrict", x => x.UserDistrictId);
                    table.ForeignKey(
                        name: "FK_UserDistrict_UserState_UserStateId",
                        column: x => x.UserStateId,
                        principalTable: "UserState",
                        principalColumn: "UserStateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPCConstituency",
                columns: table => new
                {
                    UserPCConstituencyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserStateId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPCConstituency", x => x.UserPCConstituencyId);
                    table.ForeignKey(
                        name: "FK_UserPCConstituency_UserState_UserStateId",
                        column: x => x.UserStateId,
                        principalTable: "UserState",
                        principalColumn: "UserStateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAssembly",
                columns: table => new
                {
                    UserAssemblyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserDistrictId = table.Column<int>(type: "integer", nullable: true),
                    UserPCConstituencyId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssembly", x => x.UserAssemblyId);
                    table.ForeignKey(
                        name: "FK_UserAssembly_UserDistrict_UserDistrictId",
                        column: x => x.UserDistrictId,
                        principalTable: "UserDistrict",
                        principalColumn: "UserDistrictId");
                    table.ForeignKey(
                        name: "FK_UserAssembly_UserPCConstituency_UserPCConstituencyId",
                        column: x => x.UserPCConstituencyId,
                        principalTable: "UserPCConstituency",
                        principalColumn: "UserPCConstituencyId");
                });

            migrationBuilder.CreateTable(
                name: "UserPSZone",
                columns: table => new
                {
                    UserPSZoneId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAssemblyId = table.Column<int>(type: "integer", nullable: true),
                    PSZoneMasterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPSZone", x => x.UserPSZoneId);
                    table.ForeignKey(
                        name: "FK_UserPSZone_UserAssembly_UserAssemblyId",
                        column: x => x.UserAssemblyId,
                        principalTable: "UserAssembly",
                        principalColumn: "UserAssemblyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAssembly_UserDistrictId",
                table: "UserAssembly",
                column: "UserDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssembly_UserPCConstituencyId",
                table: "UserAssembly",
                column: "UserPCConstituencyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDistrict_UserStateId",
                table: "UserDistrict",
                column: "UserStateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPCConstituency_UserStateId",
                table: "UserPCConstituency",
                column: "UserStateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPSZone_UserAssemblyId",
                table: "UserPSZone",
                column: "UserAssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserState_Id",
                table: "UserState",
                column: "Id");
        }
    }
}

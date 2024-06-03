using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class CountingandPSO5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountingLocationMaster",
                columns: table => new
                {
                    LocationMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    PcMasterId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LocationName = table.Column<int>(type: "integer", nullable: false),
                    CountingLocationStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountingLocationMaster", x => x.LocationMasterId);
                    table.ForeignKey(
                        name: "FK_CountingLocationMaster_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId");
                    table.ForeignKey(
                        name: "FK_CountingLocationMaster_ParliamentConstituencyMaster_PcMaste~",
                        column: x => x.PcMasterId,
                        principalTable: "ParliamentConstituencyMaster",
                        principalColumn: "PCMasterId");
                    table.ForeignKey(
                        name: "FK_CountingLocationMaster_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountingTypeMaster",
                columns: table => new
                {
                    CountingTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountingType = table.Column<string>(type: "text", nullable: false),
                    CountingDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CountingStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountingTypeMaster", x => x.CountingTypeMasterId);
                });

            migrationBuilder.CreateTable(
                name: "ElectionTypeMaster",
                columns: table => new
                {
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElectionType = table.Column<string>(type: "text", nullable: false),
                    ElectionStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionTypeMaster", x => x.ElectionTypeMasterId);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationMaster",
                columns: table => new
                {
                    PollingStationMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    PCasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: true),
                    ParliamentaryConstituencyNo = table.Column<string>(type: "text", nullable: false),
                    ParliamentaryConstituencyName = table.Column<string>(type: "text", nullable: false),
                    AssemblySegmentNo = table.Column<string>(type: "text", nullable: false),
                    AssemblySegmentName = table.Column<string>(type: "text", nullable: false),
                    PollingStationNo = table.Column<string>(type: "text", nullable: false),
                    PollingStationName = table.Column<string>(type: "text", nullable: false),
                    PollingStationAuxy = table.Column<string>(type: "text", nullable: false),
                    TotalCUsUsed = table.Column<int>(type: "integer", nullable: false),
                    TotalBUsUsed = table.Column<int>(type: "integer", nullable: false),
                    TotalVVPATUsed = table.Column<int>(type: "integer", nullable: false),
                    EVMReplaced = table.Column<bool>(type: "boolean", nullable: false),
                    EVMReplacementTime = table.Column<string>(type: "text", nullable: false),
                    EVMReplacementReason = table.Column<string>(type: "text", nullable: false),
                    VVPATReplaced = table.Column<bool>(type: "boolean", nullable: false),
                    VVPATReplacementTime = table.Column<string>(type: "text", nullable: false),
                    VVPATReplacementReason = table.Column<string>(type: "text", nullable: false),
                    PollingAgents = table.Column<int>(type: "integer", nullable: false),
                    VisuallyImpaired = table.Column<int>(type: "integer", nullable: false),
                    HearingImpaired = table.Column<int>(type: "integer", nullable: false),
                    LocoMotive = table.Column<int>(type: "integer", nullable: false),
                    PWDDisabilityOthers = table.Column<int>(type: "integer", nullable: false),
                    DummyBSB = table.Column<int>(type: "integer", nullable: false),
                    WHC = table.Column<int>(type: "integer", nullable: false),
                    WBF = table.Column<int>(type: "integer", nullable: false),
                    VotePolledEPIC = table.Column<int>(type: "integer", nullable: false),
                    VotePolledOtherDocument = table.Column<string>(type: "text", nullable: false),
                    TenderedVote = table.Column<int>(type: "integer", nullable: false),
                    ChallengedVote = table.Column<int>(type: "integer", nullable: false),
                    ProxyVote = table.Column<int>(type: "integer", nullable: false),
                    IsWebCastingDone = table.Column<bool>(type: "boolean", nullable: false),
                    IsWebCastingOperatorAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    WebCastingName = table.Column<string>(type: "text", nullable: false),
                    WebCastingMobileNumber = table.Column<string>(type: "text", nullable: false),
                    PSManagedByPwD = table.Column<bool>(type: "boolean", nullable: false),
                    PinkPSIsManagedByWomen = table.Column<bool>(type: "boolean", nullable: false),
                    IsModelStation = table.Column<bool>(type: "boolean", nullable: false),
                    IPresidingOfficerAgree = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationMaster", x => x.PollingStationMasterId);
                });

            migrationBuilder.CreateTable(
                name: "CountingVenueMaster",
                columns: table => new
                {
                    CountingVenueMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationMasterId = table.Column<int>(type: "integer", nullable: false),
                    CountingVenue = table.Column<string>(type: "text", nullable: false),
                    CountingVenueStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountingVenueMaster", x => x.CountingVenueMasterId);
                    table.ForeignKey(
                        name: "FK_CountingVenueMaster_CountingLocationMaster_LocationMasterId",
                        column: x => x.LocationMasterId,
                        principalTable: "CountingLocationMaster",
                        principalColumn: "LocationMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionConductedMaster",
                columns: table => new
                {
                    ElectionConductedMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionConductedStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionConductedMaster", x => x.ElectionConductedMasterId);
                    table.ForeignKey(
                        name: "FK_ElectionConductedMaster_ElectionTypeMaster_ElectionTypeMast~",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationGender",
                columns: table => new
                {
                    PollingStationGenderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PollingStationMasterId = table.Column<int>(type: "integer", nullable: false),
                    Male = table.Column<int>(type: "integer", nullable: false),
                    Female = table.Column<int>(type: "integer", nullable: false),
                    ThirdGender = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationGender", x => x.PollingStationGenderId);
                    table.ForeignKey(
                        name: "FK_PollingStationGender_PollingStationMaster_PollingStationMas~",
                        column: x => x.PollingStationMasterId,
                        principalTable: "PollingStationMaster",
                        principalColumn: "PollingStationMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountingBasicInfoMaster",
                columns: table => new
                {
                    CountingBasicInfoMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElectionConductedMasterId = table.Column<int>(type: "integer", nullable: false),
                    CountingTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    PcMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    NoOfTable = table.Column<int>(type: "integer", nullable: false),
                    NoOfRound = table.Column<int>(type: "integer", nullable: false),
                    TotalBooth = table.Column<int>(type: "integer", nullable: false),
                    CountingBasicInfoStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountingBasicInfoMaster", x => x.CountingBasicInfoMasterId);
                    table.ForeignKey(
                        name: "FK_CountingBasicInfoMaster_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountingBasicInfoMaster_CountingTypeMaster_CountingTypeMast~",
                        column: x => x.CountingTypeMasterId,
                        principalTable: "CountingTypeMaster",
                        principalColumn: "CountingTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountingBasicInfoMaster_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId");
                    table.ForeignKey(
                        name: "FK_CountingBasicInfoMaster_ElectionConductedMaster_ElectionCon~",
                        column: x => x.ElectionConductedMasterId,
                        principalTable: "ElectionConductedMaster",
                        principalColumn: "ElectionConductedMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountingBasicInfoMaster_ParliamentConstituencyMaster_PcMast~",
                        column: x => x.PcMasterId,
                        principalTable: "ParliamentConstituencyMaster",
                        principalColumn: "PCMasterId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountingBasicInfoMaster_AssemblyMasterId",
                table: "CountingBasicInfoMaster",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingBasicInfoMaster_CountingTypeMasterId",
                table: "CountingBasicInfoMaster",
                column: "CountingTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingBasicInfoMaster_DistrictMasterId",
                table: "CountingBasicInfoMaster",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingBasicInfoMaster_ElectionConductedMasterId",
                table: "CountingBasicInfoMaster",
                column: "ElectionConductedMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingBasicInfoMaster_PcMasterId",
                table: "CountingBasicInfoMaster",
                column: "PcMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingLocationMaster_DistrictMasterId",
                table: "CountingLocationMaster",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingLocationMaster_PcMasterId",
                table: "CountingLocationMaster",
                column: "PcMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingLocationMaster_StateMasterId",
                table: "CountingLocationMaster",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CountingVenueMaster_LocationMasterId",
                table: "CountingVenueMaster",
                column: "LocationMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionConductedMaster_ElectionTypeMasterId",
                table: "ElectionConductedMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationGender_PollingStationMasterId",
                table: "PollingStationGender",
                column: "PollingStationMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountingBasicInfoMaster");

            migrationBuilder.DropTable(
                name: "CountingVenueMaster");

            migrationBuilder.DropTable(
                name: "PollingStationGender");

            migrationBuilder.DropTable(
                name: "CountingTypeMaster");

            migrationBuilder.DropTable(
                name: "ElectionConductedMaster");

            migrationBuilder.DropTable(
                name: "CountingLocationMaster");

            migrationBuilder.DropTable(
                name: "PollingStationMaster");

            migrationBuilder.DropTable(
                name: "ElectionTypeMaster");
        }
    }
}

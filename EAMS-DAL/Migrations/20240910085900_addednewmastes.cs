using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addednewmastes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    OTPGeneratedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTP = table.Column<string>(type: "text", nullable: true),
                    OTPExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTPAttempts = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BLOMaster",
                columns: table => new
                {
                    BLOMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BLOMobile = table.Column<string>(type: "text", nullable: false),
                    BLOName = table.Column<string>(type: "text", nullable: false),
                    BLOStatus = table.Column<bool>(type: "boolean", nullable: false),
                    BLOCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BLOUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTPGeneratedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTP = table.Column<string>(type: "text", nullable: true),
                    OTPExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTPAttempts = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppPin = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOMaster", x => x.BLOMasterId);
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
                name: "DashboardConnectedUser",
                columns: table => new
                {
                    DashboardConnectedUserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConnectionId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    UserConnectedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardConnectedUser", x => x.DashboardConnectedUserId);
                });

            migrationBuilder.CreateTable(
                name: "ElectionInfoMaster",
                columns: table => new
                {
                    ElectionInfoMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    EventMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionInfoCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ElectionInfoUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ElectionInfoDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ElectionInfoStatus = table.Column<bool>(type: "boolean", nullable: true),
                    SOUserId = table.Column<string>(type: "text", nullable: true),
                    AROUserId = table.Column<string>(type: "text", nullable: true),
                    IsPartyDispatched = table.Column<bool>(type: "boolean", nullable: true),
                    PartyDispatchedLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPartyReached = table.Column<bool>(type: "boolean", nullable: true),
                    PartyReachedLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSetupOfPolling = table.Column<bool>(type: "boolean", nullable: true),
                    SetupOfPollingLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsMockPollDone = table.Column<bool>(type: "boolean", nullable: true),
                    MockPollDoneLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NoOfPollingAgents = table.Column<int>(type: "integer", nullable: true),
                    IsPollStarted = table.Column<bool>(type: "boolean", nullable: true),
                    PollStartedLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinalTVote = table.Column<int>(type: "integer", nullable: true),
                    FinalTVoteStatus = table.Column<bool>(type: "boolean", nullable: true),
                    VotingLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VoterInQueue = table.Column<int>(type: "integer", nullable: true),
                    VoterInQueueLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsVoterTurnOut = table.Column<bool>(type: "boolean", nullable: true),
                    VotingTurnOutLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPollEnded = table.Column<bool>(type: "boolean", nullable: true),
                    IsPollEndedLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsMCESwitchOff = table.Column<bool>(type: "boolean", nullable: true),
                    MCESwitchOffLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPartyDeparted = table.Column<bool>(type: "boolean", nullable: true),
                    PartyDepartedLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPartyReachedCollectionCenter = table.Column<bool>(type: "boolean", nullable: true),
                    PartyReachedCollectionCenterLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEVMDeposited = table.Column<bool>(type: "boolean", nullable: true),
                    EVMDepositedLastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Male = table.Column<int>(type: "integer", nullable: true),
                    Female = table.Column<int>(type: "integer", nullable: true),
                    Transgender = table.Column<int>(type: "integer", nullable: true),
                    EDC = table.Column<int>(type: "integer", nullable: true),
                    IsQueueUndo = table.Column<bool>(type: "boolean", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionInfoMaster", x => x.ElectionInfoMasterId);
                });

            migrationBuilder.CreateTable(
                name: "ElectionTypeMaster",
                columns: table => new
                {
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElectionType = table.Column<string>(type: "text", nullable: false),
                    ElectionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    Hierarchy1 = table.Column<string>(type: "text", nullable: true),
                    Hierarchy2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionTypeMaster", x => x.ElectionTypeMasterId);
                });

            migrationBuilder.CreateTable(
                name: "EventMaster",
                columns: table => new
                {
                    EventMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    EventSequence = table.Column<int>(type: "integer", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMaster", x => x.EventMasterId);
                });

            migrationBuilder.CreateTable(
                name: "FieldOfficerMaster",
                columns: table => new
                {
                    FieldOfficerMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FieldOfficerName = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerDesignation = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerOfficeName = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerMobile = table.Column<string>(type: "text", nullable: false),
                    FieldOfficerCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FieldOfficerUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FieldOfficerStatus = table.Column<bool>(type: "boolean", nullable: false),
                    OTPGeneratedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTP = table.Column<string>(type: "text", nullable: true),
                    OTPExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OTPAttempts = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppPin = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOfficerMaster", x => x.FieldOfficerMasterId);
                });

            migrationBuilder.CreateTable(
                name: "GPVoter",
                columns: table => new
                {
                    GPVoterMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    GPVoterPdfPath = table.Column<string>(type: "text", nullable: true),
                    GPVoterCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPVoterUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPVoterDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPVoterStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPVoter", x => x.GPVoterMasterId);
                });

            migrationBuilder.CreateTable(
                name: "HelpDeskDetail",
                columns: table => new
                {
                    HelpDeskMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: true),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    MobileNumber = table.Column<string>(type: "text", nullable: false),
                    LandlineNumber = table.Column<int>(type: "integer", nullable: true),
                    IsStatus = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HelpDeskStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpDeskDetail", x => x.HelpDeskMasterId);
                });

            migrationBuilder.CreateTable(
                name: "Kyc",
                columns: table => new
                {
                    KycMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    PSZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false),
                    GPPanchayatWardsMasterId = table.Column<int>(type: "integer", nullable: false),
                    CandidateName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    NominationPdfPath = table.Column<string>(type: "text", nullable: false),
                    Option1 = table.Column<string>(type: "text", nullable: true),
                    Option2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kyc", x => x.KycMasterId);
                });

            migrationBuilder.CreateTable(
                name: "MobileVersion",
                columns: table => new
                {
                    MobileVersionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    CurrentMobileVersion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileVersion", x => x.MobileVersionId);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserType = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    NotificationTime = table.Column<string>(type: "text", nullable: false),
                    NotificationStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "PPR",
                columns: table => new
                {
                    PPRMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    RandomizationTaskDetailMasterId = table.Column<int>(type: "integer", nullable: false),
                    CurrentRound = table.Column<int>(type: "integer", nullable: false),
                    DateOfRound = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfCompletedRound = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfPostponedRound = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPR", x => x.PPRMasterId);
                });

            migrationBuilder.CreateTable(
                name: "PollDetails",
                columns: table => new
                {
                    PollDetailMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlotManagementId = table.Column<int>(type: "integer", nullable: true),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    EventMasterId = table.Column<int>(type: "integer", nullable: false),
                    VotesPolled = table.Column<int>(type: "integer", nullable: false),
                    AddedBy = table.Column<int>(type: "integer", nullable: false),
                    VotesPolledRecivedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Male = table.Column<int>(type: "integer", nullable: true),
                    Female = table.Column<int>(type: "integer", nullable: true),
                    Transgender = table.Column<int>(type: "integer", nullable: true),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollDetails", x => x.PollDetailMasterId);
                });

            migrationBuilder.CreateTable(
                name: "PollInterruptionHistory",
                columns: table => new
                {
                    PollInterruptionHisId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    InterruptionType = table.Column<int>(type: "integer", nullable: false),
                    OldCU = table.Column<string>(type: "text", nullable: false),
                    OldBU = table.Column<string>(type: "text", nullable: false),
                    NewCU = table.Column<string>(type: "text", nullable: false),
                    NewBU = table.Column<string>(type: "text", nullable: false),
                    StopTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ResumeTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserType = table.Column<string>(type: "text", nullable: false),
                    UserRole = table.Column<string>(type: "text", nullable: false),
                    IsPollInterrupted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Flag = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollInterruptionHistory", x => x.PollInterruptionHisId);
                });

            migrationBuilder.CreateTable(
                name: "PollInterruptions",
                columns: table => new
                {
                    PollInterruptionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    InterruptionType = table.Column<int>(type: "integer", nullable: false),
                    OldCU = table.Column<string>(type: "text", nullable: true),
                    OldBU = table.Column<string>(type: "text", nullable: true),
                    NewCU = table.Column<string>(type: "text", nullable: true),
                    NewBU = table.Column<string>(type: "text", nullable: true),
                    StopTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ResumeTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: true),
                    UserRole = table.Column<string>(type: "text", nullable: true),
                    IsPollInterrupted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Flag = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollInterruptions", x => x.PollInterruptionId);
                });

            migrationBuilder.CreateTable(
                name: "PollingLocationMaster",
                columns: table => new
                {
                    LocationMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    LocationName = table.Column<string>(type: "text", nullable: false),
                    LocationCode = table.Column<string>(type: "text", nullable: false),
                    LocationLatitude = table.Column<string>(type: "text", nullable: true),
                    LocationLongitude = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingLocationMaster", x => x.LocationMasterId);
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
                    StateName = table.Column<string>(type: "text", nullable: false),
                    StateCode = table.Column<string>(type: "text", nullable: false),
                    DistrictName = table.Column<string>(type: "text", nullable: false),
                    DistrictCode = table.Column<string>(type: "text", nullable: false),
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
                    IPresidingOfficerAgree = table.Column<bool>(type: "boolean", nullable: false),
                    Freezed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationMaster", x => x.PollingStationMasterId);
                });

            migrationBuilder.CreateTable(
                name: "QIS",
                columns: table => new
                {
                    QueueMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    BLOMasterId = table.Column<int>(type: "integer", nullable: false),
                    BLOMobileNumber = table.Column<string>(type: "text", nullable: false),
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false),
                    BoothCode = table.Column<string>(type: "text", nullable: true),
                    QueueEnterTime = table.Column<string>(type: "text", nullable: true),
                    QueueEnterDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QueueCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QIS", x => x.QueueMasterId);
                });

            migrationBuilder.CreateTable(
                name: "RandomizationTaskDetail",
                columns: table => new
                {
                    RandomizationTaskDetailMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    TaskName = table.Column<string>(type: "text", nullable: false),
                    NumberOfRound = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RandomizationTaskDetail", x => x.RandomizationTaskDetailMasterId);
                });

            migrationBuilder.CreateTable(
                name: "ResultDeclaration",
                columns: table => new
                {
                    ResultDeclarationMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: true),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    GPPanchayatWardsMasterId = table.Column<int>(type: "integer", nullable: true),
                    CandidateName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    VoteMargin = table.Column<string>(type: "text", nullable: true),
                    ResultDecCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultDecStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDeclaration", x => x.ResultDeclarationMasterId);
                });

            migrationBuilder.CreateTable(
                name: "SMSTemplate",
                columns: table => new
                {
                    SMSTemplateMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SMSName = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    TemplateId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSTemplate", x => x.SMSTemplateMasterId);
                });

            migrationBuilder.CreateTable(
                name: "SlotManagementMaster",
                columns: table => new
                {
                    SlotManagementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    EventMasterId = table.Column<int>(type: "integer", nullable: false),
                    SlotSequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    LockTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    IsLastSlot = table.Column<bool>(type: "boolean", nullable: false),
                    SlotCreatedTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotManagementMaster", x => x.SlotManagementId);
                });

            migrationBuilder.CreateTable(
                name: "StateMaster",
                columns: table => new
                {
                    StateMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateName = table.Column<string>(type: "text", nullable: false),
                    StateCode = table.Column<string>(type: "text", nullable: false),
                    StateCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StateUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StateDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true),
                    StateStatus = table.Column<bool>(type: "boolean", nullable: false),
                    IsGenderCapturedinVoterTurnOut = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMaster", x => x.StateMasterId);
                });

            migrationBuilder.CreateTable(
                name: "UnOpposed",
                columns: table => new
                {
                    UnOpposedMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: true),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: true),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    PSZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false),
                    GPPanchayatWardsMasterId = table.Column<int>(type: "integer", nullable: false),
                    CandidateName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    NominationPdfPath = table.Column<string>(type: "text", nullable: false),
                    Option1 = table.Column<string>(type: "text", nullable: true),
                    Option2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnOpposed", x => x.UnOpposedMasterId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
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
                name: "SMSSent",
                columns: table => new
                {
                    SMSSentMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SMSTemplateMasterId = table.Column<int>(type: "integer", nullable: true),
                    Mobile = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    RemarksFromGW = table.Column<string>(type: "text", nullable: true),
                    SentToUserType = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSSent", x => x.SMSSentMasterId);
                    table.ForeignKey(
                        name: "FK_SMSSent_SMSTemplate_SMSTemplateMasterId",
                        column: x => x.SMSTemplateMasterId,
                        principalTable: "SMSTemplate",
                        principalColumn: "SMSTemplateMasterId");
                });

            migrationBuilder.CreateTable(
                name: "DistrictMaster",
                columns: table => new
                {
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictName = table.Column<string>(type: "text", nullable: false),
                    DistrictCode = table.Column<string>(type: "text", nullable: false),
                    DistrictCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DistrictUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DistrictDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DistrictStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistrictMaster", x => x.DistrictMasterId);
                    table.ForeignKey(
                        name: "FK_DistrictMaster_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParliamentConstituencyMaster",
                columns: table => new
                {
                    PCMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeId = table.Column<int>(type: "integer", nullable: false),
                    PcCodeNo = table.Column<string>(type: "text", nullable: true),
                    PcName = table.Column<string>(type: "text", nullable: true),
                    PcType = table.Column<string>(type: "text", nullable: true),
                    PcCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PcUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PcDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PcStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParliamentConstituencyMaster", x => x.PCMasterId);
                    table.ForeignKey(
                        name: "FK_ParliamentConstituencyMaster_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyMaster",
                columns: table => new
                {
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssemblyName = table.Column<string>(type: "text", nullable: false),
                    AssemblyCode = table.Column<int>(type: "integer", nullable: false),
                    AssemblyType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    PCMasterId = table.Column<int>(type: "integer", nullable: true),
                    TotalBooths = table.Column<int>(type: "integer", nullable: true),
                    AssemblyCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssemblyUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssemblyDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssemblyStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyMaster", x => x.AssemblyMasterId);
                    table.ForeignKey(
                        name: "FK_AssemblyMaster_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyMaster_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyMaster_ParliamentConstituencyMaster_PCMasterId",
                        column: x => x.PCMasterId,
                        principalTable: "ParliamentConstituencyMaster",
                        principalColumn: "PCMasterId");
                    table.ForeignKey(
                        name: "FK_AssemblyMaster_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "FourthLevelH",
                columns: table => new
                {
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HierarchyName = table.Column<string>(type: "text", nullable: true),
                    HierarchyCode = table.Column<int>(type: "integer", nullable: false),
                    HierarchyType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    HierarchyCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HierarchyUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HierarchyDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HierarchyStatus = table.Column<bool>(type: "boolean", nullable: false),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FourthLevelH", x => x.FourthLevelHMasterId);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourthLevelH_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "GPPanchayatWards",
                columns: table => new
                {
                    GPPanchayatWardsMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GPPanchayatWardsName = table.Column<string>(type: "text", nullable: false),
                    GPPanchayatWardsCode = table.Column<int>(type: "integer", nullable: false),
                    GPPanchayatWardsType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    GPPanchayatWardsCategory = table.Column<string>(type: "text", nullable: true),
                    GPPanchayatWardsCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPPanchayatWardsUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPPanchayatWardsDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPPanchayatWardsStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPPanchayatWards", x => x.GPPanchayatWardsMasterId);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GPPanchayatWards_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PSZonePanchayat",
                columns: table => new
                {
                    PSZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PSZonePanchayatName = table.Column<string>(type: "text", nullable: false),
                    PSZonePanchayatCode = table.Column<int>(type: "integer", nullable: false),
                    PSZonePanchayatType = table.Column<string>(type: "text", nullable: true),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: false),
                    PSZonePanchayatBooths = table.Column<int>(type: "integer", nullable: false),
                    PSZonePanchayatCategory = table.Column<string>(type: "text", nullable: true),
                    PSZonePanchayatCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZonePanchayatUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZonePanchayatDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PSZonePanchayatStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSZonePanchayat", x => x.PSZonePanchayatMasterId);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PSZonePanchayat_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoothMaster",
                columns: table => new
                {
                    BoothMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    DistrictMasterId = table.Column<int>(type: "integer", nullable: false),
                    AssemblyMasterId = table.Column<int>(type: "integer", nullable: false),
                    ElectionTypeMasterId = table.Column<int>(type: "integer", nullable: false),
                    FourthLevelHMasterId = table.Column<int>(type: "integer", nullable: true),
                    PSZonePanchayatMasterId = table.Column<int>(type: "integer", nullable: true),
                    BoothCode_No = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    TotalVoters = table.Column<int>(type: "integer", nullable: true),
                    Male = table.Column<int>(type: "integer", nullable: true),
                    Female = table.Column<int>(type: "integer", nullable: true),
                    Transgender = table.Column<int>(type: "integer", nullable: true),
                    BoothName = table.Column<string>(type: "text", nullable: true),
                    SecondLanguage = table.Column<string>(type: "text", nullable: true),
                    BoothNoAuxy = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    BoothStatus = table.Column<bool>(type: "boolean", nullable: false),
                    BoothCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BoothUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BoothDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedTo = table.Column<string>(type: "text", nullable: true),
                    AssignedToBLO = table.Column<string>(type: "text", nullable: true),
                    AssignedBy = table.Column<string>(type: "text", nullable: true),
                    AssignedOnTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAssigned = table.Column<bool>(type: "boolean", nullable: false),
                    LocationMasterId = table.Column<int>(type: "integer", nullable: true),
                    IsPSOFormDone = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrimaryBooth = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoothMaster", x => x.BoothMasterId);
                    table.ForeignKey(
                        name: "FK_BoothMaster_AssemblyMaster_AssemblyMasterId",
                        column: x => x.AssemblyMasterId,
                        principalTable: "AssemblyMaster",
                        principalColumn: "AssemblyMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoothMaster_DistrictMaster_DistrictMasterId",
                        column: x => x.DistrictMasterId,
                        principalTable: "DistrictMaster",
                        principalColumn: "DistrictMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoothMaster_ElectionTypeMaster_ElectionTypeMasterId",
                        column: x => x.ElectionTypeMasterId,
                        principalTable: "ElectionTypeMaster",
                        principalColumn: "ElectionTypeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoothMaster_FourthLevelH_FourthLevelHMasterId",
                        column: x => x.FourthLevelHMasterId,
                        principalTable: "FourthLevelH",
                        principalColumn: "FourthLevelHMasterId");
                    table.ForeignKey(
                        name: "FK_BoothMaster_PSZonePanchayat_PSZonePanchayatMasterId",
                        column: x => x.PSZonePanchayatMasterId,
                        principalTable: "PSZonePanchayat",
                        principalColumn: "PSZonePanchayatMasterId");
                    table.ForeignKey(
                        name: "FK_BoothMaster_StateMaster_StateMasterId",
                        column: x => x.StateMasterId,
                        principalTable: "StateMaster",
                        principalColumn: "StateMasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyMaster_DistrictMasterId",
                table: "AssemblyMaster",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyMaster_ElectionTypeMasterId",
                table: "AssemblyMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyMaster_PCMasterId",
                table: "AssemblyMaster",
                column: "PCMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyMaster_StateMasterId",
                table: "AssemblyMaster",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_AssemblyMasterId",
                table: "BoothMaster",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_DistrictMasterId",
                table: "BoothMaster",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_ElectionTypeMasterId",
                table: "BoothMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_FourthLevelHMasterId",
                table: "BoothMaster",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_PSZonePanchayatMasterId",
                table: "BoothMaster",
                column: "PSZonePanchayatMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothMaster_StateMasterId",
                table: "BoothMaster",
                column: "StateMasterId");

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
                name: "IX_DistrictMaster_StateMasterId",
                table: "DistrictMaster",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionConductedMaster_ElectionTypeMasterId",
                table: "ElectionConductedMaster",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_AssemblyMasterId",
                table: "FourthLevelH",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_DistrictMasterId",
                table: "FourthLevelH",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_ElectionTypeMasterId",
                table: "FourthLevelH",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthLevelH_StateMasterId",
                table: "FourthLevelH",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_AssemblyMasterId",
                table: "GPPanchayatWards",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_DistrictMasterId",
                table: "GPPanchayatWards",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_ElectionTypeMasterId",
                table: "GPPanchayatWards",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_FourthLevelHMasterId",
                table: "GPPanchayatWards",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_GPPanchayatWards_StateMasterId",
                table: "GPPanchayatWards",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_AssemblyMasterId",
                table: "PSZonePanchayat",
                column: "AssemblyMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_DistrictMasterId",
                table: "PSZonePanchayat",
                column: "DistrictMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_ElectionTypeMasterId",
                table: "PSZonePanchayat",
                column: "ElectionTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_FourthLevelHMasterId",
                table: "PSZonePanchayat",
                column: "FourthLevelHMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PSZonePanchayat_StateMasterId",
                table: "PSZonePanchayat",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ParliamentConstituencyMaster_StateMasterId",
                table: "ParliamentConstituencyMaster",
                column: "StateMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationGender_PollingStationMasterId",
                table: "PollingStationGender",
                column: "PollingStationMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SMSSent_SMSTemplateMasterId",
                table: "SMSSent",
                column: "SMSTemplateMasterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BLOMaster");

            migrationBuilder.DropTable(
                name: "BoothMaster");

            migrationBuilder.DropTable(
                name: "CountingBasicInfoMaster");

            migrationBuilder.DropTable(
                name: "CountingVenueMaster");

            migrationBuilder.DropTable(
                name: "DashboardConnectedUser");

            migrationBuilder.DropTable(
                name: "ElectionInfoMaster");

            migrationBuilder.DropTable(
                name: "EventMaster");

            migrationBuilder.DropTable(
                name: "FieldOfficerMaster");

            migrationBuilder.DropTable(
                name: "GPPanchayatWards");

            migrationBuilder.DropTable(
                name: "GPVoter");

            migrationBuilder.DropTable(
                name: "HelpDeskDetail");

            migrationBuilder.DropTable(
                name: "Kyc");

            migrationBuilder.DropTable(
                name: "MobileVersion");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PPR");

            migrationBuilder.DropTable(
                name: "PollDetails");

            migrationBuilder.DropTable(
                name: "PollInterruptionHistory");

            migrationBuilder.DropTable(
                name: "PollInterruptions");

            migrationBuilder.DropTable(
                name: "PollingLocationMaster");

            migrationBuilder.DropTable(
                name: "PollingStationGender");

            migrationBuilder.DropTable(
                name: "QIS");

            migrationBuilder.DropTable(
                name: "RandomizationTaskDetail");

            migrationBuilder.DropTable(
                name: "ResultDeclaration");

            migrationBuilder.DropTable(
                name: "SMSSent");

            migrationBuilder.DropTable(
                name: "SlotManagementMaster");

            migrationBuilder.DropTable(
                name: "UnOpposed");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PSZonePanchayat");

            migrationBuilder.DropTable(
                name: "CountingTypeMaster");

            migrationBuilder.DropTable(
                name: "ElectionConductedMaster");

            migrationBuilder.DropTable(
                name: "CountingLocationMaster");

            migrationBuilder.DropTable(
                name: "PollingStationMaster");

            migrationBuilder.DropTable(
                name: "SMSTemplate");

            migrationBuilder.DropTable(
                name: "FourthLevelH");

            migrationBuilder.DropTable(
                name: "AssemblyMaster");

            migrationBuilder.DropTable(
                name: "DistrictMaster");

            migrationBuilder.DropTable(
                name: "ElectionTypeMaster");

            migrationBuilder.DropTable(
                name: "ParliamentConstituencyMaster");

            migrationBuilder.DropTable(
                name: "StateMaster");
        }
    }
}

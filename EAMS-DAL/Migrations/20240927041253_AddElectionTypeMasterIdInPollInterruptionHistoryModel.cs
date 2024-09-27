using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddElectionTypeMasterIdInPollInterruptionHistoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PCMasterId",
                table: "PollInterruptions");

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "PollInterruptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "PollInterruptionHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVoterInQueue",
                table: "ElectionInfoMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "PollInterruptions");

            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "PollInterruptionHistory");

            migrationBuilder.AddColumn<int>(
                name: "PCMasterId",
                table: "PollInterruptions",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVoterInQueue",
                table: "ElectionInfoMaster",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}

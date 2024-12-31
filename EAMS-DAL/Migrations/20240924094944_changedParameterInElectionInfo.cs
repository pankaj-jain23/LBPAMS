using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedParameterInElectionInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsQueueUndo",
                table: "ElectionInfoMaster",
                newName: "IsVoterInQueue");

            migrationBuilder.RenameColumn(
                name: "FinalTVoteStatus",
                table: "ElectionInfoMaster",
                newName: "IsFinalVote");

            migrationBuilder.RenameColumn(
                name: "FinalTVote",
                table: "ElectionInfoMaster",
                newName: "FinalVote");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVoterInQueue",
                table: "ElectionInfoMaster",
                newName: "IsQueueUndo");

            migrationBuilder.RenameColumn(
                name: "IsFinalVote",
                table: "ElectionInfoMaster",
                newName: "FinalTVoteStatus");

            migrationBuilder.RenameColumn(
                name: "FinalVote",
                table: "ElectionInfoMaster",
                newName: "FinalTVote");
        }
    }
}

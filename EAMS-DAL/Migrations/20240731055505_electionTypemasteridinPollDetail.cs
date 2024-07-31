using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class electionTypemasteridinPollDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionTypeMasterId",
                table: "PollDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionTypeMasterId",
                table: "PollDetails");
        }
    }
}

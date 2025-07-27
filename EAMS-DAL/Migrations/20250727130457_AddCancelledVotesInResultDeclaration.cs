using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelledVotesInResultDeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CancelledVotes",
                table: "ResultDeclarationHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelledVotes",
                table: "ResultDeclaration",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledVotes",
                table: "ResultDeclarationHistory");

            migrationBuilder.DropColumn(
                name: "CancelledVotes",
                table: "ResultDeclaration");
        }
    }
}

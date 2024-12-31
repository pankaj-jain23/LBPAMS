using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedElectionInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Female",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "Male",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "PCMasterId",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "Transgender",
                table: "PollDetails");

            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "PollDetails",
                newName: "EventName");

            migrationBuilder.RenameColumn(
                name: "AddedBy",
                table: "PollDetails",
                newName: "EventSequence");

            migrationBuilder.AlterColumn<int>(
                name: "VotesPolled",
                table: "PollDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "EventABBR",
                table: "PollDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EventStatus",
                table: "PollDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventABBR",
                table: "PollDetails");

            migrationBuilder.DropColumn(
                name: "EventStatus",
                table: "PollDetails");

            migrationBuilder.RenameColumn(
                name: "EventSequence",
                table: "PollDetails",
                newName: "AddedBy");

            migrationBuilder.RenameColumn(
                name: "EventName",
                table: "PollDetails",
                newName: "UserType");

            migrationBuilder.AlterColumn<int>(
                name: "VotesPolled",
                table: "PollDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Female",
                table: "PollDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                table: "PollDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PCMasterId",
                table: "PollDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transgender",
                table: "PollDetails",
                type: "integer",
                nullable: true);
        }
    }
}

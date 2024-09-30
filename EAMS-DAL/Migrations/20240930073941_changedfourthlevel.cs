using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedfourthlevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "ResultDeclaration");

            migrationBuilder.RenameColumn(
                name: "CandidateType",
                table: "ResultDeclaration",
                newName: "KycMasterId");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "FourthLevelH",
                newName: "AssignedToRO");

            migrationBuilder.AddColumn<string>(
                name: "AssginedType",
                table: "FourthLevelH",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToARO",
                table: "FourthLevelH",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssginedType",
                table: "FourthLevelH");

            migrationBuilder.DropColumn(
                name: "AssignedToARO",
                table: "FourthLevelH");

            migrationBuilder.RenameColumn(
                name: "KycMasterId",
                table: "ResultDeclaration",
                newName: "CandidateType");

            migrationBuilder.RenameColumn(
                name: "AssignedToRO",
                table: "FourthLevelH",
                newName: "AssignedTo");

            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "ResultDeclaration",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

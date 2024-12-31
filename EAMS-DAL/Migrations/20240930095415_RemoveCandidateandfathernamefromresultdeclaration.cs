using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCandidateandfathernamefromresultdeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateName",
                table: "ResultDeclaration");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "ResultDeclaration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CandidateName",
                table: "ResultDeclaration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "ResultDeclaration",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

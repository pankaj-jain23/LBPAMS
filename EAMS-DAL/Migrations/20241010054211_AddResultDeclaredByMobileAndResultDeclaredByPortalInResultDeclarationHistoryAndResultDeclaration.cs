using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddResultDeclaredByMobileAndResultDeclaredByPortalInResultDeclarationHistoryAndResultDeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultDeclaredByMobile",
                table: "ResultDeclarationHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultDeclaredByPortal",
                table: "ResultDeclarationHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultDeclaredByMobile",
                table: "ResultDeclaration",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultDeclaredByPortal",
                table: "ResultDeclaration",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultDeclaredByMobile",
                table: "ResultDeclarationHistory");

            migrationBuilder.DropColumn(
                name: "ResultDeclaredByPortal",
                table: "ResultDeclarationHistory");

            migrationBuilder.DropColumn(
                name: "ResultDeclaredByMobile",
                table: "ResultDeclaration");

            migrationBuilder.DropColumn(
                name: "ResultDeclaredByPortal",
                table: "ResultDeclaration");
        }
    }
}

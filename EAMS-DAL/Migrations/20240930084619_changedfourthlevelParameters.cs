using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedfourthlevelParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAssigned",
                table: "FourthLevelH",
                newName: "IsAssignedRO");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                table: "FourthLevelH",
                newName: "ROAssignedBy");

            migrationBuilder.AddColumn<string>(
                name: "AROAssignedBy",
                table: "FourthLevelH",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAssignedARO",
                table: "FourthLevelH",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AROAssignedBy",
                table: "FourthLevelH");

            migrationBuilder.DropColumn(
                name: "IsAssignedARO",
                table: "FourthLevelH");

            migrationBuilder.RenameColumn(
                name: "ROAssignedBy",
                table: "FourthLevelH",
                newName: "AssignedBy");

            migrationBuilder.RenameColumn(
                name: "IsAssignedRO",
                table: "FourthLevelH",
                newName: "IsAssigned");
        }
    }
}

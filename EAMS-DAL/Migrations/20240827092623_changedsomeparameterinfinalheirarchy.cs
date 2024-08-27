using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class changedsomeparameterinfinalheirarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatUpdatedAt",
                table: "PSZonePanchayat",
                newName: "PSZonePanchayatUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatStatus",
                table: "PSZonePanchayat",
                newName: "PSZonePanchayatStatus");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatDeletedAt",
                table: "PSZonePanchayat",
                newName: "PSZonePanchayatDeletedAt");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatCreatedAt",
                table: "PSZonePanchayat",
                newName: "PSZonePanchayatCreatedAt");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatCategory",
                table: "PSZonePanchayat",
                newName: "PSZonePanchayatCategory");

            migrationBuilder.RenameColumn(
                name: "BlockZonePanchayatBooths",
                table: "PSZonePanchayat",
                newName: "PSZonePanchayatBooths");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsUpdatedAt",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsType",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsType");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsStatus",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsStatus");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsName",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsName");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsDeletedAt",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsDeletedAt");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsCreatedAt",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsCreatedAt");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsCode",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsCode");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsCategory",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsCategory");

            migrationBuilder.RenameColumn(
                name: "SarpanchWardsMasterId",
                table: "GPPanchayatWards",
                newName: "GPPanchayatWardsMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatUpdatedAt",
                table: "PSZonePanchayat",
                newName: "BlockZonePanchayatUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatStatus",
                table: "PSZonePanchayat",
                newName: "BlockZonePanchayatStatus");

            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatDeletedAt",
                table: "PSZonePanchayat",
                newName: "BlockZonePanchayatDeletedAt");

            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatCreatedAt",
                table: "PSZonePanchayat",
                newName: "BlockZonePanchayatCreatedAt");

            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatCategory",
                table: "PSZonePanchayat",
                newName: "BlockZonePanchayatCategory");

            migrationBuilder.RenameColumn(
                name: "PSZonePanchayatBooths",
                table: "PSZonePanchayat",
                newName: "BlockZonePanchayatBooths");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsUpdatedAt",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsType",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsType");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsStatus",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsStatus");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsName",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsName");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsDeletedAt",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsDeletedAt");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsCreatedAt",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsCreatedAt");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsCode",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsCode");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsCategory",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsCategory");

            migrationBuilder.RenameColumn(
                name: "GPPanchayatWardsMasterId",
                table: "GPPanchayatWards",
                newName: "SarpanchWardsMasterId");
        }
    }
}

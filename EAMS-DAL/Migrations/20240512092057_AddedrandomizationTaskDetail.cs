using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedrandomizationTaskDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssemblyCode",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "AssemblyName",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "CO_R1",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "CO_R2",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "CO_R3",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "MO_R1",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "MO_R2",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "PCCode",
                table: "PPR");

            migrationBuilder.RenameColumn(
                name: "PP_R3",
                table: "PPR",
                newName: "DateOfRound");

            migrationBuilder.RenameColumn(
                name: "PP_R2",
                table: "PPR",
                newName: "DateOfPostponedRound");

            migrationBuilder.RenameColumn(
                name: "PP_R1",
                table: "PPR",
                newName: "DateOfCompletedRound");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRounds",
                table: "PPR",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskMasterId",
                table: "PPR",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RandomizationTaskDetail",
                columns: table => new
                {
                    RandomizationTaskDetailMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateMasterId = table.Column<int>(type: "integer", nullable: false),
                    TaskName = table.Column<string>(type: "text", nullable: false),
                    NumberOfRound = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RandomizationTaskDetail", x => x.RandomizationTaskDetailMasterId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RandomizationTaskDetail");

            migrationBuilder.DropColumn(
                name: "NumberOfRounds",
                table: "PPR");

            migrationBuilder.DropColumn(
                name: "TaskMasterId",
                table: "PPR");

            migrationBuilder.RenameColumn(
                name: "DateOfRound",
                table: "PPR",
                newName: "PP_R3");

            migrationBuilder.RenameColumn(
                name: "DateOfPostponedRound",
                table: "PPR",
                newName: "PP_R2");

            migrationBuilder.RenameColumn(
                name: "DateOfCompletedRound",
                table: "PPR",
                newName: "PP_R1");

            migrationBuilder.AddColumn<string>(
                name: "AssemblyCode",
                table: "PPR",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssemblyName",
                table: "PPR",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CO_R1",
                table: "PPR",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CO_R2",
                table: "PPR",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CO_R3",
                table: "PPR",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "PPR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "PPR",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "MO_R1",
                table: "PPR",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MO_R2",
                table: "PPR",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PCCode",
                table: "PPR",
                type: "text",
                nullable: true);
        }
    }
}

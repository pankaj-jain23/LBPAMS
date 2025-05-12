using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations.SMS
{
    /// <inheritdoc />
    public partial class AddedSMSConfiguration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OTPSendTime",
                table: "SMSNumbers",
                newName: "SendTime");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SMSNumbers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SMSNumbers");

            migrationBuilder.RenameColumn(
                name: "SendTime",
                table: "SMSNumbers",
                newName: "OTPSendTime");
        }
    }
}

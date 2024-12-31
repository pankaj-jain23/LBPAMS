using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAMS_DAL.Migrations
{ 
    public partial class updatedResultDeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to handle type change in PostgreSQL
            migrationBuilder.Sql(
                @"ALTER TABLE ""ResultDeclarationHistory"" 
                  ALTER COLUMN ""VoteMargin"" TYPE integer USING NULL;"
            );

            migrationBuilder.Sql(
                @"ALTER TABLE ""ResultDeclaration"" 
                  ALTER COLUMN ""VoteMargin"" TYPE integer USING NULL;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the change to original type
            migrationBuilder.Sql(
                @"ALTER TABLE ""ResultDeclarationHistory"" 
                  ALTER COLUMN ""VoteMargin"" TYPE text;"
            );

            migrationBuilder.Sql(
                @"ALTER TABLE ""ResultDeclaration"" 
                  ALTER COLUMN ""VoteMargin"" TYPE text;"
            );
        }
    }
}

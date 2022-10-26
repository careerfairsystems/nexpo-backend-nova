using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class programmeinsteadofguild2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Guild",
                table: "Students",
                newName: "Programme");

            migrationBuilder.RenameColumn(
                name: "DesiredGuilds",
                table: "Companies",
                newName: "DesiredProgramme");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Programme",
                table: "Students",
                newName: "Guild");

            migrationBuilder.RenameColumn(
                name: "DesiredProgramme",
                table: "Companies",
                newName: "DesiredGuilds");
        }
    }
}

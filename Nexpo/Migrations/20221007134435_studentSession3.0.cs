using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class studentSession30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Booked",
                table: "StudentSessionApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Booked",
                table: "StudentSessionApplications");
        }
    }
}

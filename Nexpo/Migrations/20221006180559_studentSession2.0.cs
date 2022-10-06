using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class studentSession20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "booked",
                table: "StudentSessionTimeslots");

            migrationBuilder.DropColumn(
                name: "booked",
                table: "StudentSessionApplications");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudentSessionTimeslots",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentSessionTimeslots");

            migrationBuilder.AddColumn<bool>(
                name: "booked",
                table: "StudentSessionTimeslots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "booked",
                table: "StudentSessionApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class adddesiredcompetences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "DesiredCompetences",
                table: "Companies",
                type: "integer[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredCompetences",
                table: "Companies");
        }
    }
}

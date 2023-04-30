using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class DaysAtArkad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<DateTime>>(
                name: "DaysAtArkad",
                table: "Companies",
                type: "timestamp without time zone[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysAtArkad",
                table: "Companies");
        }
    }
}

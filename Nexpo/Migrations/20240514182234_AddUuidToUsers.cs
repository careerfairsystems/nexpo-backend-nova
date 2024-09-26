using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class AddUuidToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");  // Assign a new UUID by default
            
            // Update existing records with new UUIDs
            migrationBuilder.Sql("UPDATE Users SET Uuid = gen_random_uuid() WHERE Uuid IS NULL");
        }
        
        

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Users");
        }
    }
}

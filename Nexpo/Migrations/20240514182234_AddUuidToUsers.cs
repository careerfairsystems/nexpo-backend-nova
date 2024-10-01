using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nexpo.Migrations
{
    public partial class AddUuidToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\";");

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");  // Assign a new UUID by default

            // Update existing records with new UUIDs
            migrationBuilder.Sql("UPDATE \"Users\" SET \"Uuid\" = gen_random_uuid() WHERE \"Uuid\" IS NULL");
            migrationBuilder.Sql("UPDATE \"Users\" SET \"Uuid\" = gen_random_uuid() WHERE \"Uuid\" = '00000000-0000-0000-0000-000000000000';");
        }
        
        

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Users");
        }
    }
}

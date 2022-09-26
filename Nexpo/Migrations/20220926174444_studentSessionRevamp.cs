using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nexpo.Migrations
{
    public partial class studentSessionRevamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessionApplications_StudentSessions_StudentSessionId",
                table: "StudentSessionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessionTimeslots_StudentSessions_StudentSessionId",
                table: "StudentSessionTimeslots");

            migrationBuilder.DropTable(
                name: "StudentSessions");

            migrationBuilder.DropIndex(
                name: "IX_StudentSessionTimeslots_StudentSessionId",
                table: "StudentSessionTimeslots");

            migrationBuilder.DropIndex(
                name: "IX_StudentSessionApplications_StudentSessionId",
                table: "StudentSessionApplications");

            migrationBuilder.DropColumn(
                name: "StudentSessionId",
                table: "StudentSessionTimeslots");

            migrationBuilder.DropColumn(
                name: "StudentSessionId",
                table: "StudentSessionApplications");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "StudentSessionApplications",
                newName: "Status");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "booked",
                table: "StudentSessionTimeslots");

            migrationBuilder.DropColumn(
                name: "booked",
                table: "StudentSessionApplications");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "StudentSessionApplications",
                newName: "Rating");

            migrationBuilder.AddColumn<int>(
                name: "StudentSessionId",
                table: "StudentSessionTimeslots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentSessionId",
                table: "StudentSessionApplications",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    StudentSessionApplicationId = table.Column<int>(type: "integer", nullable: false),
                    StudentSessionTimeslotId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSessions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSessions_StudentSessionApplications_StudentSessionAp~",
                        column: x => x.StudentSessionApplicationId,
                        principalTable: "StudentSessionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSessions_StudentSessionTimeslots_StudentSessionTimes~",
                        column: x => x.StudentSessionTimeslotId,
                        principalTable: "StudentSessionTimeslots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessionTimeslots_StudentSessionId",
                table: "StudentSessionTimeslots",
                column: "StudentSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessionApplications_StudentSessionId",
                table: "StudentSessionApplications",
                column: "StudentSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessions_StudentId",
                table: "StudentSessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessions_StudentSessionApplicationId",
                table: "StudentSessions",
                column: "StudentSessionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessions_StudentSessionTimeslotId",
                table: "StudentSessions",
                column: "StudentSessionTimeslotId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessionApplications_StudentSessions_StudentSessionId",
                table: "StudentSessionApplications",
                column: "StudentSessionId",
                principalTable: "StudentSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessionTimeslots_StudentSessions_StudentSessionId",
                table: "StudentSessionTimeslots",
                column: "StudentSessionId",
                principalTable: "StudentSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

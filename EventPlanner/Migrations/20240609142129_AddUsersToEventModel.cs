using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations
{
    public partial class AddUsersToEventModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "AppUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_EventId",
                table: "AppUsers",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Event_EventId",
                table: "AppUsers",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Event_EventId",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_EventId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "AppUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations
{
    public partial class AddManyUsersToManyEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "AppUserEvent",
                columns: table => new
                {
                    EventsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserEvent", x => new { x.EventsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AppUserEvent_AppUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserEvent_Event_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserEvent_UsersId",
                table: "AppUserEvent",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserEvent");

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
    }
}

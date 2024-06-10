using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations
{
    public partial class AddAuthorEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Event_AuthorId",
                table: "Event",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AppUsers_AuthorId",
                table: "Event",
                column: "AuthorId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_AppUsers_AuthorId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_AuthorId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Event");
        }
    }
}

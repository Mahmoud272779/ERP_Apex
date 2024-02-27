using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class notificationsystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                table: "NotificationSeen");

            migrationBuilder.DropIndex(
                name: "IX_NotificationSeen_NotificationsMasterId",
                table: "NotificationSeen");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeen_NotificationsMasterId",
                table: "NotificationSeen",
                column: "NotificationsMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                table: "NotificationSeen",
                column: "NotificationsMasterId",
                principalTable: "NotificationsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToUserToBugReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserId",
                table: "BugReports",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_AssignedToUserId",
                table: "BugReports",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_AspNetUsers_AssignedToUserId",
                table: "BugReports",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_AspNetUsers_AssignedToUserId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_AssignedToUserId",
                table: "BugReports");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "BugReports");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePc2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "BugReports",
                newName: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_StatusId",
                table: "BugReports",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_BugStatuses_StatusId",
                table: "BugReports",
                column: "StatusId",
                principalTable: "BugStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_BugStatuses_StatusId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_StatusId",
                table: "BugReports");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "BugReports",
                newName: "Status");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBugReferenceFromBugReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_Bugs_BugId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_BugId",
                table: "BugReports");

            migrationBuilder.DropColumn(
                name: "BugId",
                table: "BugReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BugId",
                table: "BugReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_BugId",
                table: "BugReports",
                column: "BugId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_Bugs_BugId",
                table: "BugReports",
                column: "BugId",
                principalTable: "Bugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_ApplicationName_ProjectId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_ProjectId",
                table: "BugReports");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "BugReports",
                newName: "PriorityId");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "BugReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_ApplicationId",
                table: "BugReports",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_ApplicationName_ApplicationId",
                table: "BugReports",
                column: "ApplicationId",
                principalTable: "ApplicationName",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_ApplicationName_ApplicationId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_ApplicationId",
                table: "BugReports");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "BugReports");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                table: "BugReports",
                newName: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_ProjectId",
                table: "BugReports",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_ApplicationName_ProjectId",
                table: "BugReports",
                column: "ProjectId",
                principalTable: "ApplicationName",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeveloperAssignmentAndAdminSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_BugStatuses_StatusId",
                table: "BugReports");

            migrationBuilder.DropTable(
                name: "Bugs");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "BugReports");

            migrationBuilder.AddColumn<string>(
                name: "DeveloperId",
                table: "BugReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BugReportId = table.Column<int>(type: "int", nullable: false),
                    SubscribedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminSubscriptions_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminSubscriptions_BugReports_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BugReportId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_BugReports_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_DeveloperId",
                table: "BugReports",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_PriorityId",
                table: "BugReports",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminSubscriptions_AdminId_BugReportId",
                table: "AdminSubscriptions",
                columns: new[] { "AdminId", "BugReportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminSubscriptions_BugReportId",
                table: "AdminSubscriptions",
                column: "BugReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BugReportId",
                table: "Notifications",
                column: "BugReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_AspNetUsers_DeveloperId",
                table: "BugReports",
                column: "DeveloperId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_BugPriorities_PriorityId",
                table: "BugReports",
                column: "PriorityId",
                principalTable: "BugPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_BugStatuses_StatusId",
                table: "BugReports",
                column: "StatusId",
                principalTable: "BugStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_AspNetUsers_DeveloperId",
                table: "BugReports");

            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_BugPriorities_PriorityId",
                table: "BugReports");

            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_BugStatuses_StatusId",
                table: "BugReports");

            migrationBuilder.DropTable(
                name: "AdminSubscriptions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_DeveloperId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_PriorityId",
                table: "BugReports");

            migrationBuilder.DropColumn(
                name: "DeveloperId",
                table: "BugReports");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "BugReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bugs", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_BugStatuses_StatusId",
                table: "BugReports",
                column: "StatusId",
                principalTable: "BugStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

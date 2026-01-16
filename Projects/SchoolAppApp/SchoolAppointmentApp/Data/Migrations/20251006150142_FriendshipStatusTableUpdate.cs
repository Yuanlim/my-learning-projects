using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FriendshipStatusTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendStatuses");

            migrationBuilder.DropTable(
                name: "FriendshipStatuses");

            migrationBuilder.CreateTable(
                name: "FriendRequestStatuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FriendRequestPossibleStatus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequestStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    InitiatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_FriendRequests_FriendRequestStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "FriendRequestStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FriendRequestStatuses",
                columns: new[] { "StatusId", "FriendRequestPossibleStatus" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Denied" },
                    { 3, "Accepted" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_InitiatorId",
                table: "FriendRequests",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReceiverId_InitiatorId",
                table: "FriendRequests",
                columns: new[] { "ReceiverId", "InitiatorId" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_StatusId",
                table: "FriendRequests",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "FriendRequestStatuses");

            migrationBuilder.CreateTable(
                name: "FriendshipStatuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FriendRequestStatus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendshipStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "FriendStatuses",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InitiatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendStatuses", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_FriendStatuses_FriendshipStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "FriendshipStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendStatuses_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendStatuses_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FriendshipStatuses",
                columns: new[] { "StatusId", "FriendRequestStatus" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Denied" },
                    { 3, "Accepted" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatuses_InitiatorId",
                table: "FriendStatuses",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatuses_ReceiverId_InitiatorId",
                table: "FriendStatuses",
                columns: new[] { "ReceiverId", "InitiatorId" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatuses_StatusId",
                table: "FriendStatuses",
                column: "StatusId");
        }
    }
}

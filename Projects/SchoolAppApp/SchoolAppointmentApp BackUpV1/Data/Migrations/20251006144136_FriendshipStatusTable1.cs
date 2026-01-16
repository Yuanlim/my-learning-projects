using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FriendshipStatusTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendshipStatus",
                table: "FriendStatuses");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "FriendStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_FriendStatuses_StatusId",
                table: "FriendStatuses",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendStatuses_FriendshipStatuses_StatusId",
                table: "FriendStatuses",
                column: "StatusId",
                principalTable: "FriendshipStatuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendStatuses_FriendshipStatuses_StatusId",
                table: "FriendStatuses");

            migrationBuilder.DropTable(
                name: "FriendshipStatuses");

            migrationBuilder.DropIndex(
                name: "IX_FriendStatuses_StatusId",
                table: "FriendStatuses");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "FriendStatuses");

            migrationBuilder.AddColumn<string>(
                name: "FriendshipStatus",
                table: "FriendStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

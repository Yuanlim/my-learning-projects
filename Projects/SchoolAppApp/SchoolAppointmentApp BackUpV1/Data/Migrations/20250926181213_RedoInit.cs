using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RedoInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Teachers_CustomerId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderItemId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReceiverId",
                table: "Reports");

            migrationBuilder.DropCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_FriendStatuses_ReceiverId",
                table: "FriendStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_ReceiverId",
                table: "Blocks");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderStatuses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderItemStatuses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "MainPosts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    PostDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NumOfTumbsUp = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainPosts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_MainPosts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    ReplyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    PostDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NumOfTumbsUp = table.Column<int>(type: "INTEGER", nullable: false),
                    MainPostPostId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.ReplyId);
                    table.ForeignKey(
                        name: "FK_Replies_MainPosts_MainPostPostId",
                        column: x => x.MainPostPostId,
                        principalTable: "MainPosts",
                        principalColumn: "PostId");
                    table.ForeignKey(
                        name: "FK_Replies_MainPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "MainPosts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TumbsUpInfos",
                columns: table => new
                {
                    TumbId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReplyId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TumbsPossibleStatus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TumbsUpInfos", x => x.TumbId);
                    table.CheckConstraint("CK_ReplyIdAndPostIdCoExist", "(\"PostId\" IS NULL AND \"ReplyId\" IS NOT NULL)OR (\"ReplyId\" IS NULL AND \"PostId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_TumbsUpInfos_MainPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "MainPosts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TumbsUpInfos_Replies_ReplyId",
                        column: x => x.ReplyId,
                        principalTable: "Replies",
                        principalColumn: "ReplyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TumbsUpInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "OrderItemStatuses",
                keyColumn: "StatusId",
                keyValue: 1,
                column: "Status",
                value: "pending");

            migrationBuilder.UpdateData(
                table: "OrderItemStatuses",
                keyColumn: "StatusId",
                keyValue: 2,
                column: "Status",
                value: "received");

            migrationBuilder.UpdateData(
                table: "OrderItemStatuses",
                keyColumn: "StatusId",
                keyValue: 3,
                column: "Status",
                value: "cancelled");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 1,
                column: "Status",
                value: "pending");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 2,
                column: "Status",
                value: "cancelled");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 3,
                column: "Status",
                value: "received");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 4,
                column: "Status",
                value: "mix");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TeacherIdAndStudentIdCoexist",
                table: "Users",
                sql: "(\"TeacherId\" IS NULL AND \"StudentId\" IS NOT NULL)OR (\"StudentId\" IS NULL AND \"TeacherId\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherId",
                table: "Teachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentId",
                table: "Students",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReceiverId_InitiatorId",
                table: "Reports",
                columns: new[] { "ReceiverId", "InitiatorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductId",
                table: "Products",
                column: "ProductId");

            migrationBuilder.AddCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages",
                sql: "(CASE WHEN \"Content\" IS NOT NULL AND length(trim(\"Content\")) > 0 THEN 1 ELSE 0 END) +(CASE WHEN \"AudioMessageRoot\" IS NOT NULL THEN 1 ELSE 0 END) +(CASE WHEN \"ImageMessageRoot\" IS NOT NULL THEN 1 ELSE 0 END) = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatuses_ReceiverId_InitiatorId",
                table: "FriendStatuses",
                columns: new[] { "ReceiverId", "InitiatorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_ReceiverId_InitiatorId",
                table: "Blocks",
                columns: new[] { "ReceiverId", "InitiatorId" });

            migrationBuilder.CreateIndex(
                name: "IX_MainPosts_StudentId",
                table: "MainPosts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_MainPostPostId",
                table: "Replies",
                column: "MainPostPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_PostId",
                table: "Replies",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_UserId",
                table: "Replies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TumbsUpInfos_PostId_UserId",
                table: "TumbsUpInfos",
                columns: new[] { "PostId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TumbsUpInfos_ReplyId_UserId",
                table: "TumbsUpInfos",
                columns: new[] { "ReplyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TumbsUpInfos_UserId",
                table: "TumbsUpInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Teachers_CustomerId",
                table: "Carts",
                column: "CustomerId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Teachers_CustomerId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "TumbsUpInfos");

            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.DropTable(
                name: "MainPosts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TeacherIdAndStudentIdCoexist",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TeacherId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReceiverId_InitiatorId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductId",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_FriendStatuses_ReceiverId_InitiatorId",
                table: "FriendStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_ReceiverId_InitiatorId",
                table: "Blocks");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "OrderStatuses",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "OrderItemStatuses",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.UpdateData(
                table: "OrderItemStatuses",
                keyColumn: "StatusId",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "OrderItemStatuses",
                keyColumn: "StatusId",
                keyValue: 2,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OrderItemStatuses",
                keyColumn: "StatusId",
                keyValue: 3,
                column: "Status",
                value: 2);

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 2,
                column: "Status",
                value: 2);

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 3,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 4,
                column: "Status",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReceiverId",
                table: "Reports",
                column: "ReceiverId");

            migrationBuilder.AddCheckConstraint(
                name: "CheckMessageHasExactlyOneTypeOfContent",
                table: "Messages",
                sql: "(CASE WHEN Content IS NOT NULL AND length(trim(Content)) > 0 THEN 1 ELSE 0 END) +(CASE WHEN AudioMessageRoot IS NOT NULL THEN 1 ELSE 0 END) +(CASE WHEN ImageMessageRoot IS NOT NULL THEN 1 ELSE 0 END) = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatuses_ReceiverId",
                table: "FriendStatuses",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_ReceiverId",
                table: "Blocks",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Teachers_CustomerId",
                table: "Carts",
                column: "CustomerId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderItemId",
                table: "OrderItems",
                column: "OrderItemId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

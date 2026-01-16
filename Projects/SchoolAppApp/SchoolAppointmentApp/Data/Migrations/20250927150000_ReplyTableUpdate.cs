using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplyTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_MainPosts_MainPostPostId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_MainPosts_PostId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_TumbsUpInfos_MainPosts_PostId",
                table: "TumbsUpInfos");

            migrationBuilder.DropIndex(
                name: "IX_Replies_MainPostPostId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "MainPostPostId",
                table: "Replies");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "TumbsUpInfos",
                newName: "MainPostId");

            migrationBuilder.RenameColumn(
                name: "TumbId",
                table: "TumbsUpInfos",
                newName: "TumbsUpInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_TumbsUpInfos_PostId_UserId",
                table: "TumbsUpInfos",
                newName: "IX_TumbsUpInfos_MainPostId_UserId");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "Replies",
                newName: "MainPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_PostId",
                table: "Replies",
                newName: "IX_Replies_MainPostId");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "MainPosts",
                newName: "MainPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_MainPosts_MainPostId",
                table: "Replies",
                column: "MainPostId",
                principalTable: "MainPosts",
                principalColumn: "MainPostId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TumbsUpInfos_MainPosts_MainPostId",
                table: "TumbsUpInfos",
                column: "MainPostId",
                principalTable: "MainPosts",
                principalColumn: "MainPostId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_MainPosts_MainPostId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_TumbsUpInfos_MainPosts_MainPostId",
                table: "TumbsUpInfos");

            migrationBuilder.RenameColumn(
                name: "MainPostId",
                table: "TumbsUpInfos",
                newName: "PostId");

            migrationBuilder.RenameColumn(
                name: "TumbsUpInfoId",
                table: "TumbsUpInfos",
                newName: "TumbId");

            migrationBuilder.RenameIndex(
                name: "IX_TumbsUpInfos_MainPostId_UserId",
                table: "TumbsUpInfos",
                newName: "IX_TumbsUpInfos_PostId_UserId");

            migrationBuilder.RenameColumn(
                name: "MainPostId",
                table: "Replies",
                newName: "PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_MainPostId",
                table: "Replies",
                newName: "IX_Replies_PostId");

            migrationBuilder.RenameColumn(
                name: "MainPostId",
                table: "MainPosts",
                newName: "PostId");

            migrationBuilder.AddColumn<int>(
                name: "MainPostPostId",
                table: "Replies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_MainPostPostId",
                table: "Replies",
                column: "MainPostPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_MainPosts_MainPostPostId",
                table: "Replies",
                column: "MainPostPostId",
                principalTable: "MainPosts",
                principalColumn: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_MainPosts_PostId",
                table: "Replies",
                column: "PostId",
                principalTable: "MainPosts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TumbsUpInfos_MainPosts_PostId",
                table: "TumbsUpInfos",
                column: "PostId",
                principalTable: "MainPosts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

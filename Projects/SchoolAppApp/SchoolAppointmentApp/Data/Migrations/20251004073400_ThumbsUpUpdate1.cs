using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ThumbsUpUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ReplyIdAndPostIdCoExist",
                table: "TumbsUpInfos");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ReplyIdAndPostIdCoExist",
                table: "TumbsUpInfos",
                sql: "(\"MainPostId\" IS NULL AND \"ReplyId\" IS NOT NULL)OR (\"ReplyId\" IS NULL AND \"MainPostId\" IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ReplyIdAndPostIdCoExist",
                table: "TumbsUpInfos");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ReplyIdAndPostIdCoExist",
                table: "TumbsUpInfos",
                sql: "(\"PostId\" IS NULL AND \"ReplyId\" IS NOT NULL)OR (\"ReplyId\" IS NULL AND \"PostId\" IS NOT NULL)");
        }
    }
}

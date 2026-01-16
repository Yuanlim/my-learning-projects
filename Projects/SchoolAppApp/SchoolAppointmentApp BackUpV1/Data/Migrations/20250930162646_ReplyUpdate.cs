using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumOfTumbsUp",
                table: "Replies",
                newName: "NumOfThumbsUp");

            migrationBuilder.RenameColumn(
                name: "NumOfTumbsUp",
                table: "MainPosts",
                newName: "NumOfThumbsUp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumOfThumbsUp",
                table: "Replies",
                newName: "NumOfTumbsUp");

            migrationBuilder.RenameColumn(
                name: "NumOfThumbsUp",
                table: "MainPosts",
                newName: "NumOfTumbsUp");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ThumbsUpTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TumbsPossibleStatus",
                table: "TumbsUpInfos");

            migrationBuilder.RenameColumn(
                name: "TumbsUpInfoId",
                table: "TumbsUpInfos",
                newName: "ThumbsUpInfoId");

            migrationBuilder.AddColumn<bool>(
                name: "Thumbed",
                table: "TumbsUpInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbed",
                table: "TumbsUpInfos");

            migrationBuilder.RenameColumn(
                name: "ThumbsUpInfoId",
                table: "TumbsUpInfos",
                newName: "TumbsUpInfoId");

            migrationBuilder.AddColumn<string>(
                name: "TumbsPossibleStatus",
                table: "TumbsUpInfos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FriendShipFixesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FriendshipStatus",
                table: "FriendStatuses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FriendshipStatus",
                table: "FriendStatuses",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}

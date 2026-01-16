using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class StudentClassTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_SchoolClasses_SchoolClassClassId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolClassClassId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SchoolClassClassId",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolClassClassId",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolClassClassId",
                table: "Students",
                column: "SchoolClassClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_SchoolClasses_SchoolClassClassId",
                table: "Students",
                column: "SchoolClassClassId",
                principalTable: "SchoolClasses",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

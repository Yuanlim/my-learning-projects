using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserInfoUniqueUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TeacherIdAndStudentIdCoexist",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_TeacherIdAndStudentIdCoexist",
                table: "Users",
                sql: "(\"TeacherId\" IS NULL AND \"StudentId\" IS NOT NULL)OR (\"StudentId\" IS NULL AND \"TeacherId\" IS NOT NULL)");
        }
    }
}

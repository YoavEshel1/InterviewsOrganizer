using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewsOrganizer.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailHashUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmailHash",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmailHash",
                table: "AspNetUsers",
                column: "EmailHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmailHash",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmailHash",
                table: "AspNetUsers",
                column: "EmailHash");
        }
    }
}

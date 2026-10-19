using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewsOrganizer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCompanyPositionInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Positions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Interviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Backfill existing rows with a valid user before adding FK constraints
            migrationBuilder.Sql(@"
                UPDATE Positions SET UserId = (SELECT TOP 1 Id FROM AspNetUsers) WHERE UserId = '';
                UPDATE Interviews SET UserId = (SELECT TOP 1 Id FROM AspNetUsers) WHERE UserId = '';
                UPDATE Companies SET UserId = (SELECT TOP 1 Id FROM AspNetUsers) WHERE UserId = '';
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_UserId",
                table: "Positions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_UserId",
                table: "Interviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_UserId",
                table: "Companies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_AspNetUsers_UserId",
                table: "Interviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_AspNetUsers_UserId",
                table: "Positions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_UserId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_AspNetUsers_UserId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_AspNetUsers_UserId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_UserId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_UserId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Companies");
        }
    }
}

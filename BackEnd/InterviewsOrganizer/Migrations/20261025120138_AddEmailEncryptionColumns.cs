using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewsOrganizer.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailEncryptionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailEncryptedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailHash",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmailHash",
                table: "AspNetUsers",
                column: "EmailHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmailHash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailEncryptedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailHash",
                table: "AspNetUsers");
        }
    }
}

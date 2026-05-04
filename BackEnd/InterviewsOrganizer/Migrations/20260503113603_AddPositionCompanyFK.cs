using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewsOrganizer.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionCompanyFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Companies_CompanyId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_CompanyId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Interviews");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CompanyId",
                table: "Positions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_PositionId",
                table: "Interviews",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Positions_PositionId",
                table: "Interviews",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Companies_CompanyId",
                table: "Positions",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Positions_PositionId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Companies_CompanyId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_CompanyId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_PositionId",
                table: "Interviews");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Interviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CompanyId",
                table: "Interviews",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Companies_CompanyId",
                table: "Interviews",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}

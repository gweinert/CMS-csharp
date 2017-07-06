using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Migrations
{
    public partial class SiteUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Sites",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_ApplicationUserId",
                table: "Sites",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_AspNetUsers_ApplicationUserId",
                table: "Sites",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_AspNetUsers_ApplicationUserId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_ApplicationUserId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Sites");
        }
    }
}

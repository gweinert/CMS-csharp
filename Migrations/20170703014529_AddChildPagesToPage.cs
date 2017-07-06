using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Migrations
{
    public partial class AddChildPagesToPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageID",
                table: "Pages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageID",
                table: "Pages",
                column: "PageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Pages_PageID",
                table: "Pages",
                column: "PageID",
                principalTable: "Pages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Pages_PageID",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PageID",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageID",
                table: "Pages");
        }
    }
}

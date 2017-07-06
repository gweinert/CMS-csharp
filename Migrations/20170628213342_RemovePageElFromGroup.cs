using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Migrations
{
    public partial class RemovePageElFromGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageElements_PageElementGroups_PageElementGroupID",
                table: "PageElements");

            migrationBuilder.DropIndex(
                name: "IX_PageElements_PageElementGroupID",
                table: "PageElements");

            migrationBuilder.DropColumn(
                name: "PageElementGroupID",
                table: "PageElements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageElementGroupID",
                table: "PageElements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageElements_PageElementGroupID",
                table: "PageElements",
                column: "PageElementGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_PageElements_PageElementGroups_PageElementGroupID",
                table: "PageElements",
                column: "PageElementGroupID",
                principalTable: "PageElementGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuthLine.Migrations
{
    public partial class _20221019003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SendTime",
                table: "LineNotifySendDt",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendTime",
                table: "LineNotifySendDt");
        }
    }
}

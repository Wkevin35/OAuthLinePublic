using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuthLine.Migrations
{
    public partial class _20221019002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LineIdentity",
                newName: "LineIdentityKey");

            migrationBuilder.CreateTable(
                name: "LineNotifySendDt",
                columns: table => new
                {
                    LineNotifySendDtKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineNotifySendMtId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LineIdentityKey = table.Column<int>(type: "INTEGER", nullable: false),
                    isSuccess = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineNotifySendDt", x => x.LineNotifySendDtKey);
                });

            migrationBuilder.CreateTable(
                name: "LineNotifySendMt",
                columns: table => new
                {
                    LineNotifySendMtId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineNotifySendMt", x => x.LineNotifySendMtId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineNotifySendDt");

            migrationBuilder.DropTable(
                name: "LineNotifySendMt");

            migrationBuilder.RenameColumn(
                name: "LineIdentityKey",
                table: "LineIdentity",
                newName: "Id");
        }
    }
}

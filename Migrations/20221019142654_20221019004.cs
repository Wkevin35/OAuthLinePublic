using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuthLine.Migrations
{
    public partial class _20221019004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LineNotifySendMt",
                table: "LineNotifySendMt");

            migrationBuilder.AlterColumn<int>(
                name: "LineNotifySendMtKey",
                table: "LineNotifySendMt",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineNotifySendMt",
                table: "LineNotifySendMt",
                column: "LineNotifySendMtKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LineNotifySendMt",
                table: "LineNotifySendMt");

            migrationBuilder.AlterColumn<int>(
                name: "LineNotifySendMtKey",
                table: "LineNotifySendMt",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineNotifySendMt",
                table: "LineNotifySendMt",
                column: "LineNotifySendMtId");
        }
    }
}

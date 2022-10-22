using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuthLine.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LineIdentity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sub = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LoginIdToken = table.Column<string>(type: "TEXT", nullable: false),
                    LoginAccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    LoginRefreshToken = table.Column<string>(type: "TEXT", nullable: false),
                    NotifyAccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    Picture = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineIdentity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineIdentity");
        }
    }
}

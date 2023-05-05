using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CMSUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    NameIdentifier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    EmailConfirmed = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Mobile = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Roles = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSUsers", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "CMSUsers",
                columns: new[] { "UserId", "Email", "EmailConfirmed", "FirstName", "LastName", "Mobile", "NameIdentifier", "Password", "Provider", "Roles", "UserName" },
                values: new object[] { 1, "junk@dabrel.com", "confirmed", "junk", "user", "", "", "junk", "Cookies", "admin", "junk@dabrel.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CMSUsers");
        }
    }
}

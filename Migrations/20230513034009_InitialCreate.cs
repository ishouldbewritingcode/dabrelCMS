using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CMSSites",
                columns: table => new
                {
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Design = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    SubTitle = table.Column<string>(type: "TEXT", nullable: true),
                    Footer1 = table.Column<string>(type: "TEXT", nullable: true),
                    Footer2 = table.Column<string>(type: "TEXT", nullable: true),
                    Footer3 = table.Column<string>(type: "TEXT", nullable: true),
                    Footer4 = table.Column<string>(type: "TEXT", nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", nullable: true),
                    MetaImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    OnAllPages = table.Column<string>(type: "TEXT", nullable: true),
                    BodyTop = table.Column<string>(type: "TEXT", nullable: true),
                    BodyBottom = table.Column<string>(type: "TEXT", nullable: true),
                    ImageFileName = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FaviconUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSSites", x => x.SiteId);
                });

            migrationBuilder.CreateTable(
                name: "CMSUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: false),
                    NameIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    EmailConfirmed = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Mobile = table.Column<string>(type: "TEXT", nullable: false),
                    Roles = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CMSPages",
                columns: table => new
                {
                    PageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    isOn = table.Column<bool>(type: "INTEGER", nullable: false),
                    isPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    isHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Shortcut = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    NavTitle = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    HeroImage = table.Column<string>(type: "TEXT", nullable: true),
                    CMSSiteSiteId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSPages", x => x.PageId);
                    table.ForeignKey(
                        name: "FK_CMSPages_CMSSites_CMSSiteSiteId",
                        column: x => x.CMSSiteSiteId,
                        principalTable: "CMSSites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "CMSSiteUrls",
                columns: table => new
                {
                    SiteUrlId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Primary = table.Column<bool>(type: "INTEGER", nullable: false),
                    CMSSiteSiteId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSSiteUrls", x => x.SiteUrlId);
                    table.ForeignKey(
                        name: "FK_CMSSiteUrls_CMSSites_CMSSiteSiteId",
                        column: x => x.CMSSiteSiteId,
                        principalTable: "CMSSites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "CMSItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: true),
                    End = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Shortcut = table.Column<string>(type: "TEXT", nullable: true),
                    Title1 = table.Column<string>(type: "TEXT", nullable: true),
                    Title2 = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    CMSPagePageId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_CMSItems_CMSPages_CMSPagePageId",
                        column: x => x.CMSPagePageId,
                        principalTable: "CMSPages",
                        principalColumn: "PageId");
                });

            migrationBuilder.InsertData(
                table: "CMSPages",
                columns: new[] { "PageId", "CMSSiteSiteId", "HeroImage", "NavTitle", "ParentId", "Shortcut", "SiteId", "Sort", "Summary", "Tags", "Title", "isHidden", "isOn", "isPrivate" },
                values: new object[] { 1, null, "", "Home", 0, "", 1, 1, "Page summary goes here", "home", "Welcome", false, true, false });

            migrationBuilder.InsertData(
                table: "CMSSiteUrls",
                columns: new[] { "SiteUrlId", "CMSSiteSiteId", "Primary", "SiteId", "Url" },
                values: new object[] { 1, null, true, 1, "localhost" });

            migrationBuilder.InsertData(
                table: "CMSSites",
                columns: new[] { "SiteId", "BodyBottom", "BodyTop", "Created", "Design", "FaviconUrl", "Footer1", "Footer2", "Footer3", "Footer4", "ImageFileName", "MetaDescription", "MetaImagePath", "Name", "OnAllPages", "SubTitle", "Title" },
                values: new object[] { 1, "", "", new DateTime(2023, 5, 12, 22, 40, 9, 168, DateTimeKind.Local).AddTicks(8354), "theblues", "", "footer 1", "footer 2", "footer 3", "footer 4", "", "Description", "", "test", "", "subtitle", "title" });

            migrationBuilder.InsertData(
                table: "CMSUsers",
                columns: new[] { "UserId", "Email", "EmailConfirmed", "FirstName", "LastName", "Mobile", "NameIdentifier", "Password", "Provider", "Roles", "SiteId", "UserName" },
                values: new object[] { 1, "test@dabrel.com", "confirmed", "test", "user", "", "", "test", "Cookies", "admin", 1, "test@dabrel.com" });

            migrationBuilder.CreateIndex(
                name: "IX_CMSItems_CMSPagePageId",
                table: "CMSItems",
                column: "CMSPagePageId");

            migrationBuilder.CreateIndex(
                name: "IX_CMSPages_CMSSiteSiteId",
                table: "CMSPages",
                column: "CMSSiteSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CMSSiteUrls_CMSSiteSiteId",
                table: "CMSSiteUrls",
                column: "CMSSiteSiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CMSItems");

            migrationBuilder.DropTable(
                name: "CMSSiteUrls");

            migrationBuilder.DropTable(
                name: "CMSUsers");

            migrationBuilder.DropTable(
                name: "CMSPages");

            migrationBuilder.DropTable(
                name: "CMSSites");
        }
    }
}

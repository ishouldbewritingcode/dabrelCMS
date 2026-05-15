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
                name: "CMSBlocks",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SiteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlockType = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Title1 = table.Column<string>(type: "TEXT", nullable: true),
                    Title2 = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSBlocks", x => x.BlockId);
                });

            migrationBuilder.CreateTable(
                name: "CMSFiles",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SiteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Filename = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSFiles", x => x.FileId);
                });

            migrationBuilder.CreateTable(
                name: "CMSSites",
                columns: table => new
                {
                    SiteId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SiteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: true),
                    NameIdentifier = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "CMSItems",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlockId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    CMSBlockBlockId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_CMSItems_CMSBlocks_CMSBlockBlockId",
                        column: x => x.CMSBlockBlockId,
                        principalTable: "CMSBlocks",
                        principalColumn: "BlockId");
                });

            migrationBuilder.CreateTable(
                name: "CMSPages",
                columns: table => new
                {
                    PageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    isOn = table.Column<bool>(type: "INTEGER", nullable: false),
                    isPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    isHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Shortcut = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    NavTitle = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    HeroImage = table.Column<string>(type: "TEXT", nullable: true),
                    CMSSiteSiteId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                    SiteUrlId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SiteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Primary = table.Column<bool>(type: "INTEGER", nullable: false),
                    CMSSiteSiteId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                name: "CMSPageBlocks",
                columns: table => new
                {
                    PageBlockID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    PageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    AltTitle = table.Column<string>(type: "TEXT", nullable: true),
                    AltSubtitle = table.Column<string>(type: "TEXT", nullable: true),
                    CMSPagePageId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSPageBlocks", x => x.PageBlockID);
                    table.ForeignKey(
                        name: "FK_CMSPageBlocks_CMSPages_CMSPagePageId",
                        column: x => x.CMSPagePageId,
                        principalTable: "CMSPages",
                        principalColumn: "PageId");
                });

            migrationBuilder.InsertData(
                table: "CMSPages",
                columns: new[] { "PageId", "CMSSiteSiteId", "HeroImage", "NavTitle", "ParentId", "Shortcut", "SiteId", "Sort", "Summary", "Tags", "Title", "isHidden", "isOn", "isPrivate" },
                values: new object[] { new Guid("00000000-0000-7000-8000-000000000004"), null, "", "Home", null, "", new Guid("00000000-0000-7000-8000-000000000001"), 1, "Page summary goes here", "home", "Welcome", false, true, false });

            migrationBuilder.InsertData(
                table: "CMSSiteUrls",
                columns: new[] { "SiteUrlId", "CMSSiteSiteId", "Primary", "SiteId", "Url" },
                values: new object[] { new Guid("00000000-0000-7000-8000-000000000002"), null, true, new Guid("00000000-0000-7000-8000-000000000001"), "localhost" });

            migrationBuilder.InsertData(
                table: "CMSSites",
                columns: new[] { "SiteId", "BodyBottom", "BodyTop", "Created", "Design", "FaviconUrl", "Footer1", "Footer2", "Footer3", "Footer4", "ImageFileName", "MetaDescription", "MetaImagePath", "Name", "OnAllPages", "SubTitle", "Title" },
                values: new object[] { new Guid("00000000-0000-7000-8000-000000000001"), "", "", new DateTime(2025, 1, 1, 0, 1, 0, 0, DateTimeKind.Unspecified), "mountain", "", "footer 1", "", "", "", "", "Description", "", "test", "", "subtitle", "title" });

            migrationBuilder.InsertData(
                table: "CMSUsers",
                columns: new[] { "UserId", "Email", "FirstName", "LastName", "Mobile", "NameIdentifier", "Password", "Provider", "Roles", "Salt", "SiteId" },
                values: new object[] { new Guid("00000000-0000-7000-8000-000000000003"), "test@dabrel.com", "test", "user", "", "", "test", "Cookies", "admin", 0, new Guid("00000000-0000-7000-8000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_CMSItems_CMSBlockBlockId",
                table: "CMSItems",
                column: "CMSBlockBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_CMSPageBlocks_CMSPagePageId",
                table: "CMSPageBlocks",
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
                name: "CMSFiles");

            migrationBuilder.DropTable(
                name: "CMSItems");

            migrationBuilder.DropTable(
                name: "CMSPageBlocks");

            migrationBuilder.DropTable(
                name: "CMSSiteUrls");

            migrationBuilder.DropTable(
                name: "CMSUsers");

            migrationBuilder.DropTable(
                name: "CMSBlocks");

            migrationBuilder.DropTable(
                name: "CMSPages");

            migrationBuilder.DropTable(
                name: "CMSSites");
        }
    }
}

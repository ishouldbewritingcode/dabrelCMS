using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class _20240926_Blocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CMSItems_CMSPages_CMSPagePageId",
                table: "CMSItems");

            migrationBuilder.RenameColumn(
                name: "PageId",
                table: "CMSItems",
                newName: "BlockId");

            migrationBuilder.RenameColumn(
                name: "CMSPagePageId",
                table: "CMSItems",
                newName: "CMSBlockBlockId");

            migrationBuilder.RenameIndex(
                name: "IX_CMSItems_CMSPagePageId",
                table: "CMSItems",
                newName: "IX_CMSItems_CMSBlockBlockId");

            migrationBuilder.CreateTable(
                name: "CMSBlocks",
                columns: table => new
                {
                    BlockId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BlockType = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    PageId = table.Column<int>(type: "INTEGER", nullable: true),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    Title1 = table.Column<string>(type: "TEXT", nullable: true),
                    Title2 = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    CMSPagePageId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSBlocks", x => x.BlockId);
                    table.ForeignKey(
                        name: "FK_CMSBlocks_CMSPages_CMSPagePageId",
                        column: x => x.CMSPagePageId,
                        principalTable: "CMSPages",
                        principalColumn: "PageId");
                });

            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                columns: new[] { "Created", "Design", "Footer2", "Footer3", "Footer4" },
                values: new object[] { new DateTime(2024, 9, 26, 22, 47, 33, 826, DateTimeKind.Local).AddTicks(3962), "superbee", "", "", "" });

            migrationBuilder.CreateIndex(
                name: "IX_CMSBlocks_CMSPagePageId",
                table: "CMSBlocks",
                column: "CMSPagePageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CMSItems_CMSBlocks_CMSBlockBlockId",
                table: "CMSItems",
                column: "CMSBlockBlockId",
                principalTable: "CMSBlocks",
                principalColumn: "BlockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CMSItems_CMSBlocks_CMSBlockBlockId",
                table: "CMSItems");

            migrationBuilder.DropTable(
                name: "CMSBlocks");

            migrationBuilder.RenameColumn(
                name: "CMSBlockBlockId",
                table: "CMSItems",
                newName: "CMSPagePageId");

            migrationBuilder.RenameColumn(
                name: "BlockId",
                table: "CMSItems",
                newName: "PageId");

            migrationBuilder.RenameIndex(
                name: "IX_CMSItems_CMSBlockBlockId",
                table: "CMSItems",
                newName: "IX_CMSItems_CMSPagePageId");

            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                columns: new[] { "Created", "Design", "Footer2", "Footer3", "Footer4" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 49, 20, 496, DateTimeKind.Local).AddTicks(5272), "theblues", "footer 2", "footer 3", "footer 4" });

            migrationBuilder.AddForeignKey(
                name: "FK_CMSItems_CMSPages_CMSPagePageId",
                table: "CMSItems",
                column: "CMSPagePageId",
                principalTable: "CMSPages",
                principalColumn: "PageId");
        }
    }
}

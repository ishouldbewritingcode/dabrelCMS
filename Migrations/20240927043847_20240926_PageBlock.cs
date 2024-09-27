using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class _20240926_PageBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageId",
                table: "CMSBlocks");

            migrationBuilder.CreateTable(
                name: "CMSPageBlocks",
                columns: table => new
                {
                    PageBlockID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageId = table.Column<int>(type: "INTEGER", nullable: false),
                    BlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSPageBlocks", x => x.PageBlockID);
                });

            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2024, 9, 26, 23, 38, 47, 77, DateTimeKind.Local).AddTicks(7469));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CMSPageBlocks");

            migrationBuilder.AddColumn<int>(
                name: "PageId",
                table: "CMSBlocks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2024, 9, 26, 22, 47, 33, 826, DateTimeKind.Local).AddTicks(3962));
        }
    }
}

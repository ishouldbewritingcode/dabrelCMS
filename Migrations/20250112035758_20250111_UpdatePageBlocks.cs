using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class _20250111_UpdatePageBlocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 1, 1, 0, 1, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 1, 11, 21, 30, 57, 927, DateTimeKind.Local).AddTicks(7208));
        }
    }
}

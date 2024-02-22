using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class _20240221_Files : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2024, 2, 21, 23, 0, 49, 641, DateTimeKind.Local).AddTicks(9159));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 12, 9, 22, 52, 0, 700, DateTimeKind.Local).AddTicks(6282));
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class AddTotpSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TotpSecret",
                table: "CMSUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CMSUsers",
                keyColumn: "UserId",
                keyValue: new Guid("00000000-0000-7000-8000-000000000003"),
                column: "TotpSecret",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotpSecret",
                table: "CMSUsers");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dabrelCMS.Migrations
{
    /// <inheritdoc />
    public partial class _20231209_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "CMSUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CMSUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "CMSUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "NameIdentifier",
                table: "CMSUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Salt",
                table: "CMSUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 12, 9, 22, 52, 0, 700, DateTimeKind.Local).AddTicks(6282));

            migrationBuilder.UpdateData(
                table: "CMSUsers",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Salt",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "CMSUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "CMSUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameIdentifier",
                table: "CMSUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmed",
                table: "CMSUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CMSUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "CMSSites",
                keyColumn: "SiteId",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 5, 18, 13, 42, 1, 161, DateTimeKind.Local).AddTicks(6184));

            migrationBuilder.UpdateData(
                table: "CMSUsers",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "EmailConfirmed", "UserName" },
                values: new object[] { "confirmed", "test@dabrel.com" });
        }
    }
}

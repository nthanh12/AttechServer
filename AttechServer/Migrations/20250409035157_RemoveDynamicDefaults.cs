using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDynamicDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "auth",
                table: "UserRole",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 4, 9, 10, 45, 46, 519, DateTimeKind.Local).AddTicks(9359));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "auth",
                table: "User",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 4, 9, 10, 45, 46, 511, DateTimeKind.Local).AddTicks(2446));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "auth",
                table: "Role",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 4, 9, 10, 45, 46, 521, DateTimeKind.Local).AddTicks(3510));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "auth",
                table: "UserRole",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 4, 9, 10, 45, 46, 519, DateTimeKind.Local).AddTicks(9359),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "auth",
                table: "User",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 4, 9, 10, 45, 46, 511, DateTimeKind.Local).AddTicks(2446),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "auth",
                table: "Role",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 4, 9, 10, 45, 46, 521, DateTimeKind.Local).AddTicks(3510),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

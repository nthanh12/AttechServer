using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class updatePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_ApiEndpoints_ApiEndpointId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_KeyPermissions_KeyPermissionId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropIndex(
                name: "IX_RolePermission",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "PermissionKey",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsAuthenticate",
                table: "PermissionForApiEndpoints");

            migrationBuilder.AddColumn<int>(
                name: "PermissionId",
                table: "RolePermissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "KeyPermissionId",
                table: "PermissionForApiEndpoints",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "PermissionForApiEndpoints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PermissionForApiEndpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "PermissionForApiEndpoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "PermissionForApiEndpoints",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "PermissionForApiEndpoints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "PermissionForApiEndpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermissionId",
                table: "PermissionForApiEndpoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PermissionId1",
                table: "PermissionForApiEndpoints",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "ApiEndpoints",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ApiEndpoints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ApiEndpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ApiEndpoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "ApiEndpoints",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "ApiEndpoints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ApiEndpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireAuthentication",
                table: "ApiEndpoints",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionKey = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    PermissionLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    OrderPriority = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Permissions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission",
                table: "RolePermissions",
                columns: new[] { "Deleted", "RoleId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionForApiEndpoint",
                table: "PermissionForApiEndpoints",
                columns: new[] { "Deleted", "ApiEndpointId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionForApiEndpoints_PermissionId",
                table: "PermissionForApiEndpoints",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionForApiEndpoints_PermissionId1",
                table: "PermissionForApiEndpoints",
                column: "PermissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_ApiEndpoint",
                table: "ApiEndpoints",
                columns: new[] { "Deleted", "Path", "HttpMethod" });

            migrationBuilder.CreateIndex(
                name: "IX_Permission",
                table: "Permissions",
                columns: new[] { "Deleted", "PermissionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ParentId",
                table: "Permissions",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoints_ApiEndpoints_ApiEndpointId",
                table: "PermissionForApiEndpoints",
                column: "ApiEndpointId",
                principalTable: "ApiEndpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoints_KeyPermissions_KeyPermissionId",
                table: "PermissionForApiEndpoints",
                column: "KeyPermissionId",
                principalTable: "KeyPermissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoints_Permissions_PermissionId",
                table: "PermissionForApiEndpoints",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoints_Permissions_PermissionId1",
                table: "PermissionForApiEndpoints",
                column: "PermissionId1",
                principalTable: "Permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_ApiEndpoints_ApiEndpointId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_KeyPermissions_KeyPermissionId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_Permissions_PermissionId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_Permissions_PermissionId1",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermission",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_PermissionForApiEndpoint",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropIndex(
                name: "IX_PermissionForApiEndpoints_PermissionId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropIndex(
                name: "IX_PermissionForApiEndpoints_PermissionId1",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropIndex(
                name: "IX_ApiEndpoint",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "PermissionId1",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "RequireAuthentication",
                table: "ApiEndpoints");

            migrationBuilder.AddColumn<string>(
                name: "PermissionKey",
                table: "RolePermissions",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "KeyPermissionId",
                table: "PermissionForApiEndpoints",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthenticate",
                table: "PermissionForApiEndpoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "ApiEndpoints",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission",
                table: "RolePermissions",
                columns: new[] { "Deleted", "RoleId", "PermissionKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoints_ApiEndpoints_ApiEndpointId",
                table: "PermissionForApiEndpoints",
                column: "ApiEndpointId",
                principalTable: "ApiEndpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoints_KeyPermissions_KeyPermissionId",
                table: "PermissionForApiEndpoints",
                column: "KeyPermissionId",
                principalTable: "KeyPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

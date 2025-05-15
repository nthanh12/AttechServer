using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class splitPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeyPermission_KeyPermission_ParentId",
                schema: "auth",
                table: "KeyPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoint_ApiEndpoint_ApiEndpointId",
                schema: "auth",
                table: "PermissionForApiEndpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoint_KeyPermission_KeyPermissionId",
                schema: "auth",
                table: "PermissionForApiEndpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostCategory_PostCategoryId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductCategory_ProductCategoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                schema: "auth",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "auth",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                schema: "auth",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                schema: "auth",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "auth",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                schema: "auth",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                schema: "auth",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategory",
                table: "ProductCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionForApiEndpoint",
                schema: "auth",
                table: "PermissionForApiEndpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KeyPermission",
                schema: "auth",
                table: "KeyPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiEndpoint",
                schema: "auth",
                table: "ApiEndpoint");

            migrationBuilder.RenameTable(
                name: "UserRole",
                schema: "auth",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "auth",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "RolePermission",
                schema: "auth",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "auth",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "ProductCategory",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "PermissionForApiEndpoint",
                schema: "auth",
                newName: "PermissionForApiEndpoints");

            migrationBuilder.RenameTable(
                name: "KeyPermission",
                schema: "auth",
                newName: "KeyPermissions");

            migrationBuilder.RenameTable(
                name: "ApiEndpoint",
                schema: "auth",
                newName: "ApiEndpoints");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_UserId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategory_Slug",
                table: "ProductCategories",
                newName: "IX_ProductCategories_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Slug",
                table: "Products",
                newName: "IX_Products_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductCategoryId",
                table: "Products",
                newName: "IX_Products_ProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_Slug",
                table: "Posts",
                newName: "IX_Posts_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Post_PostCategoryId",
                table: "Posts",
                newName: "IX_Posts_PostCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionForApiEndpoint_KeyPermissionId",
                table: "PermissionForApiEndpoints",
                newName: "IX_PermissionForApiEndpoints_KeyPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionForApiEndpoint_ApiEndpointId",
                table: "PermissionForApiEndpoints",
                newName: "IX_PermissionForApiEndpoints_ApiEndpointId");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "PostCategory",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PostCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "ProductCategories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "KeyPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionForApiEndpoints",
                table: "PermissionForApiEndpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeyPermissions",
                table: "KeyPermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiEndpoints",
                table: "ApiEndpoints",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_Type",
                table: "PostCategory",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Type",
                table: "Posts",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_KeyPermissions_KeyPermissions_ParentId",
                table: "KeyPermissions",
                column: "ParentId",
                principalTable: "KeyPermissions",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostCategory_PostCategoryId",
                table: "Posts",
                column: "PostCategoryId",
                principalTable: "PostCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeyPermissions_KeyPermissions_ParentId",
                table: "KeyPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_ApiEndpoints_ApiEndpointId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionForApiEndpoints_KeyPermissions_KeyPermissionId",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostCategory_PostCategoryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_ProductCategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_PostCategory_Type",
                table: "PostCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Type",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionForApiEndpoints",
                table: "PermissionForApiEndpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KeyPermissions",
                table: "KeyPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiEndpoints",
                table: "ApiEndpoints");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PostCategory");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Posts");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRole",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "RolePermission",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "ProductCategory");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "PermissionForApiEndpoints",
                newName: "PermissionForApiEndpoint",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "KeyPermissions",
                newName: "KeyPermission",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ApiEndpoints",
                newName: "ApiEndpoint",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserId",
                schema: "auth",
                table: "UserRole",
                newName: "IX_UserRole_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                schema: "auth",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "auth",
                table: "RolePermission",
                newName: "IX_RolePermission_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Slug",
                table: "Product",
                newName: "IX_Product_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Product",
                newName: "IX_Product_ProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_Slug",
                table: "ProductCategory",
                newName: "IX_ProductCategory_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_Slug",
                table: "Post",
                newName: "IX_Post_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_PostCategoryId",
                table: "Post",
                newName: "IX_Post_PostCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionForApiEndpoints_KeyPermissionId",
                schema: "auth",
                table: "PermissionForApiEndpoint",
                newName: "IX_PermissionForApiEndpoint_KeyPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionForApiEndpoints_ApiEndpointId",
                schema: "auth",
                table: "PermissionForApiEndpoint",
                newName: "IX_PermissionForApiEndpoint_ApiEndpointId");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Services",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "PostCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                schema: "auth",
                table: "User",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Product",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "ProductCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Post",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                schema: "auth",
                table: "KeyPermission",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "auth",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                schema: "auth",
                table: "UserRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                schema: "auth",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                schema: "auth",
                table: "RolePermission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategory",
                table: "ProductCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionForApiEndpoint",
                schema: "auth",
                table: "PermissionForApiEndpoint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeyPermission",
                schema: "auth",
                table: "KeyPermission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiEndpoint",
                schema: "auth",
                table: "ApiEndpoint",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KeyPermission_KeyPermission_ParentId",
                schema: "auth",
                table: "KeyPermission",
                column: "ParentId",
                principalSchema: "auth",
                principalTable: "KeyPermission",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoint_ApiEndpoint_ApiEndpointId",
                schema: "auth",
                table: "PermissionForApiEndpoint",
                column: "ApiEndpointId",
                principalSchema: "auth",
                principalTable: "ApiEndpoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionForApiEndpoint_KeyPermission_KeyPermissionId",
                schema: "auth",
                table: "PermissionForApiEndpoint",
                column: "KeyPermissionId",
                principalSchema: "auth",
                principalTable: "KeyPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostCategory_PostCategoryId",
                table: "Post",
                column: "PostCategoryId",
                principalTable: "PostCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductCategory_ProductCategoryId",
                table: "Product",
                column: "ProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                schema: "auth",
                table: "RolePermission",
                column: "RoleId",
                principalSchema: "auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "auth",
                table: "UserRole",
                column: "RoleId",
                principalSchema: "auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                schema: "auth",
                table: "UserRole",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

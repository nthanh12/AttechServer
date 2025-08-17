using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class updateSeedDataV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsCategory_NewsCategoryId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_NotificationCategory_NotificationCategoryId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationCategory",
                table: "NotificationCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsCategory",
                table: "NewsCategory");

            migrationBuilder.RenameTable(
                name: "NotificationCategory",
                newName: "NotificationCategories");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "NewsCategory",
                newName: "NewsCategories");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationCategory_SlugVi",
                table: "NotificationCategories",
                newName: "IX_NotificationCategories_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationCategory_SlugEn",
                table: "NotificationCategories",
                newName: "IX_NotificationCategories_SlugEn");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SlugVi",
                table: "Notifications",
                newName: "IX_Notifications_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SlugEn",
                table: "Notifications",
                newName: "IX_Notifications_SlugEn");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_NotificationCategoryId",
                table: "Notifications",
                newName: "IX_Notifications_NotificationCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsCategory_SlugVi",
                table: "NewsCategories",
                newName: "IX_NewsCategories_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_NewsCategory_SlugEn",
                table: "NewsCategories",
                newName: "IX_NewsCategories_SlugEn");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "Services",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "Services",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "Products",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "Products",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "ProductCategories",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "ProductCategories",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "News",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "News",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "News",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "NotificationCategories",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "NotificationCategories",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "NotificationCategories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "Notifications",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "Notifications",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "NewsCategories",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "NewsCategories",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "NewsCategories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationCategories",
                table: "NotificationCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsCategories",
                table: "NewsCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsCategories_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId",
                principalTable: "NewsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationCategories_NotificationCategoryId",
                table: "Notifications",
                column: "NotificationCategoryId",
                principalTable: "NotificationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsCategories_NewsCategoryId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationCategories_NotificationCategoryId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationCategories",
                table: "NotificationCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsCategories",
                table: "NewsCategories");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "NotificationCategories",
                newName: "NotificationCategory");

            migrationBuilder.RenameTable(
                name: "NewsCategories",
                newName: "NewsCategory");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SlugVi",
                table: "Notification",
                newName: "IX_Notification_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SlugEn",
                table: "Notification",
                newName: "IX_Notification_SlugEn");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_NotificationCategoryId",
                table: "Notification",
                newName: "IX_Notification_NotificationCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationCategories_SlugVi",
                table: "NotificationCategory",
                newName: "IX_NotificationCategory_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationCategories_SlugEn",
                table: "NotificationCategory",
                newName: "IX_NotificationCategory_SlugEn");

            migrationBuilder.RenameIndex(
                name: "IX_NewsCategories_SlugVi",
                table: "NewsCategory",
                newName: "IX_NewsCategory_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_NewsCategories_SlugEn",
                table: "NewsCategory",
                newName: "IX_NewsCategory_SlugEn");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "Services",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "Services",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "Products",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "Products",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "ProductCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "ProductCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "News",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "News",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "News",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "Notification",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "Notification",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Notification",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "NotificationCategory",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "NotificationCategory",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "NotificationCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVi",
                table: "NewsCategory",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "NewsCategory",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(700)",
                oldMaxLength: 700);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "NewsCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationCategory",
                table: "NotificationCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsCategory",
                table: "NewsCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsCategory_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId",
                principalTable: "NewsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_NotificationCategory_NotificationCategoryId",
                table: "Notification",
                column: "NotificationCategoryId",
                principalTable: "NotificationCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

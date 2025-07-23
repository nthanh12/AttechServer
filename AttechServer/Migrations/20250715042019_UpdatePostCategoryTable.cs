using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategory_PostCategory_ParentId",
                table: "PostCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostCategory_PostCategoryId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Slug",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategory",
                table: "PostCategory");

            migrationBuilder.RenameTable(
                name: "PostCategory",
                newName: "PostCategories");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Services",
                newName: "SlugVi");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Services",
                newName: "SlugEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Services",
                newName: "DescriptionVi");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Services",
                newName: "ImageUrl");

            migrationBuilder.RenameIndex(
                name: "IX_Services_Slug",
                table: "Services",
                newName: "IX_Services_SlugVi");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Products",
                newName: "SlugVi");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "SlugEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "DescriptionVi");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Slug",
                table: "Products",
                newName: "IX_Products_SlugVi");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "ProductCategories",
                newName: "SlugVi");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductCategories",
                newName: "SlugEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ProductCategories",
                newName: "DescriptionVi");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_Slug",
                table: "ProductCategories",
                newName: "IX_ProductCategories_SlugVi");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Posts",
                newName: "TitleVi");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Posts",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Posts",
                newName: "DescriptionVi");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Posts",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "PostCategories",
                newName: "SlugVi");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PostCategories",
                newName: "SlugEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PostCategories",
                newName: "DescriptionVi");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategory_Type",
                table: "PostCategories",
                newName: "IX_PostCategories_Type");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategory_Slug",
                table: "PostCategories",
                newName: "IX_PostCategories_SlugVi");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategory_ParentId",
                table: "PostCategories",
                newName: "IX_PostCategories_ParentId");

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentVi",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Services",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Services",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameVi",
                table: "Services",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentVi",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Products",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameVi",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "ProductCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "ProductCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameVi",
                table: "ProductCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentVi",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Posts",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SlugEn",
                table: "Posts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SlugVi",
                table: "Posts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "PostCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "PostCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameVi",
                table: "PostCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Services_SlugEn",
                table: "Services",
                column: "SlugEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SlugEn",
                table: "Products",
                column: "SlugEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_SlugEn",
                table: "ProductCategories",
                column: "SlugEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SlugEn",
                table: "Posts",
                column: "SlugEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SlugVi",
                table: "Posts",
                column: "SlugVi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_SlugEn",
                table: "PostCategories",
                column: "SlugEn",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategories_PostCategories_ParentId",
                table: "PostCategories",
                column: "ParentId",
                principalTable: "PostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostCategories_PostCategoryId",
                table: "Posts",
                column: "PostCategoryId",
                principalTable: "PostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategories_PostCategories_ParentId",
                table: "PostCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostCategories_PostCategoryId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Services_SlugEn",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Products_SlugEn",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_SlugEn",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Posts_SlugEn",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_SlugVi",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories");

            migrationBuilder.DropIndex(
                name: "IX_PostCategories_SlugEn",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ContentVi",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NameVi",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ContentVi",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameVi",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "NameVi",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ContentVi",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SlugEn",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SlugVi",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "NameVi",
                table: "PostCategories");

            migrationBuilder.RenameTable(
                name: "PostCategories",
                newName: "PostCategory");

            migrationBuilder.RenameColumn(
                name: "SlugVi",
                table: "Services",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "SlugEn",
                table: "Services",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Services",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "Services",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Services_SlugVi",
                table: "Services",
                newName: "IX_Services_Slug");

            migrationBuilder.RenameColumn(
                name: "SlugVi",
                table: "Products",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "SlugEn",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Products_SlugVi",
                table: "Products",
                newName: "IX_Products_Slug");

            migrationBuilder.RenameColumn(
                name: "SlugVi",
                table: "ProductCategories",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "SlugEn",
                table: "ProductCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "ProductCategories",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_SlugVi",
                table: "ProductCategories",
                newName: "IX_ProductCategories_Slug");

            migrationBuilder.RenameColumn(
                name: "TitleVi",
                table: "Posts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Posts",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Posts",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "Posts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "SlugVi",
                table: "PostCategory",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "SlugEn",
                table: "PostCategory",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "PostCategory",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategories_Type",
                table: "PostCategory",
                newName: "IX_PostCategory_Type");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategories_SlugVi",
                table: "PostCategory",
                newName: "IX_PostCategory_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategories_ParentId",
                table: "PostCategory",
                newName: "IX_PostCategory_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategory",
                table: "PostCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategory_PostCategory_ParentId",
                table: "PostCategory",
                column: "ParentId",
                principalTable: "PostCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostCategory_PostCategoryId",
                table: "Posts",
                column: "PostCategoryId",
                principalTable: "PostCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class addSlugToPostCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostPCategory");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PostCategory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PostCategory",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "PostCategory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Post",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Post",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Post",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_Slug",
                table: "PostCategory",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Slug",
                table: "Post",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostCategory_Slug",
                table: "PostCategory");

            migrationBuilder.DropIndex(
                name: "IX_Post_Slug",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PostCategory");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "PostCategory");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PostCategory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.CreateTable(
                name: "PostPCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostCategoryId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostPCategory_PostCategory_PostCategoryId",
                        column: x => x.PostCategoryId,
                        principalTable: "PostCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostPCategory_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostPCategory",
                table: "PostPCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostPCategory_PostCategoryId",
                table: "PostPCategory",
                column: "PostCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostPCategory_PostId",
                table: "PostPCategory",
                column: "PostId");
        }
    }
}

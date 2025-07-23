using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class AddSelfReferenceToPostCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "PostCategory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_ParentId",
                table: "PostCategory",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategory_PostCategory_ParentId",
                table: "PostCategory",
                column: "ParentId",
                principalTable: "PostCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategory_PostCategory_ParentId",
                table: "PostCategory");

            migrationBuilder.DropIndex(
                name: "IX_PostCategory_ParentId",
                table: "PostCategory");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "PostCategory");
        }
    }
}

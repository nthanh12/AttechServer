using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttechServer.Migrations
{
    /// <inheritdoc />
    public partial class changeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TitleVi",
                table: "Services",
                newName: "TitleVi");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Services",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "TitleVi",
                table: "Products",
                newName: "TitleVi");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Products",
                newName: "TitleEn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TitleVi",
                table: "Services",
                newName: "TitleVi");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Services",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "TitleVi",
                table: "Products",
                newName: "TitleVi");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Products",
                newName: "TitleEn");
        }
    }
}

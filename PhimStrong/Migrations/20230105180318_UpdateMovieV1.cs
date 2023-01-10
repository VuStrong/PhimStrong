using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class UpdateMovieV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryMovie_Categories_CategoryId",
                table: "CategoryMovie");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "CategoryMovie",
                newName: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryMovie_Categories_CategoriesId",
                table: "CategoryMovie",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryMovie_Categories_CategoriesId",
                table: "CategoryMovie");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryMovie",
                newName: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryMovie_Categories_CategoryId",
                table: "CategoryMovie",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

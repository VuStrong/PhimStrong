using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class ModifyTrailerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Trailer_TrailerId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Trailer_Movies_MovieId",
                table: "Trailer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trailer",
                table: "Trailer");

            migrationBuilder.RenameTable(
                name: "Trailer",
                newName: "Trailers");

            migrationBuilder.RenameIndex(
                name: "IX_Trailer_MovieId",
                table: "Trailers",
                newName: "IX_Trailers_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trailers",
                table: "Trailers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Trailers_TrailerId",
                table: "Movies",
                column: "TrailerId",
                principalTable: "Trailers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trailers_Movies_MovieId",
                table: "Trailers",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Trailers_TrailerId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Trailers_Movies_MovieId",
                table: "Trailers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trailers",
                table: "Trailers");

            migrationBuilder.RenameTable(
                name: "Trailers",
                newName: "Trailer");

            migrationBuilder.RenameIndex(
                name: "IX_Trailers_MovieId",
                table: "Trailer",
                newName: "IX_Trailer_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trailer",
                table: "Trailer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Trailer_TrailerId",
                table: "Movies",
                column: "TrailerId",
                principalTable: "Trailer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trailer_Movies_MovieId",
                table: "Trailer",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

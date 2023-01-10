using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class AddTrailerToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trailer",
                table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "TrailerId",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Trailer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Clip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trailer_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_TrailerId",
                table: "Movies",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailer_MovieId",
                table: "Trailer",
                column: "MovieId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Trailer_TrailerId",
                table: "Movies",
                column: "TrailerId",
                principalTable: "Trailer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Trailer_TrailerId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "Trailer");

            migrationBuilder.DropIndex(
                name: "IX_Movies_TrailerId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "TrailerId",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "Trailer",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

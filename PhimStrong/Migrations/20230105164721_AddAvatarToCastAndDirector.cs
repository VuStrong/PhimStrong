using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class AddAvatarToCastAndDirector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Directors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Casts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Directors");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Casts");
        }
    }
}

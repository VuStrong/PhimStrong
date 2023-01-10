using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class UpdateCountryV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                table: "Countries");
        }
    }
}

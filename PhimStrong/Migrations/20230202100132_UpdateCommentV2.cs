using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class UpdateCommentV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropForeignKey(
				name: "FK_Comments_Comments_ResponseToId",
				table: "Comments");

			migrationBuilder.AddForeignKey(
				name: "FK_Comments_Comments_ResponseToId",
				table: "Comments",
				column: "ResponseToId",
				principalTable: "Comments",
				principalColumn: "Id",
				onDelete: ReferentialAction.NoAction);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

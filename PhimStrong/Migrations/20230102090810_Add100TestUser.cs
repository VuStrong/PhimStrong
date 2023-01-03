using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhimStrong.Migrations
{
    public partial class Add100TestUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 50; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[]
                    {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "DisplayName"
                    },
                    values: new object[]
                    {
                        Guid.NewGuid().ToString(),
                        "TestUser" + i.ToString(),
                        "email" + i.ToString() + "@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0,
                        "Test User " + i.ToString()
                    }
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

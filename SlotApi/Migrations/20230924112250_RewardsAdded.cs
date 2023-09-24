using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotApi.Migrations
{
    public partial class RewardsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyRewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscordUserDiscordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Claimed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyRewards_User_DiscordUserDiscordId",
                        column: x => x.DiscordUserDiscordId,
                        principalTable: "User",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyRewards_DiscordUserDiscordId",
                table: "DailyRewards",
                column: "DiscordUserDiscordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyRewards");
        }
    }
}

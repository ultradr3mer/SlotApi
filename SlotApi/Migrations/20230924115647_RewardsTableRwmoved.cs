using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotApi.Migrations
{
    public partial class RewardsTableRwmoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyRewards");

            migrationBuilder.AddColumn<DateTime>(
                name: "DailyRewardLast",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DailyRewardStreak",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyRewardLast",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DailyRewardStreak",
                table: "User");

            migrationBuilder.CreateTable(
                name: "DailyRewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscordUserDiscordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Claimed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StreakId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
    }
}

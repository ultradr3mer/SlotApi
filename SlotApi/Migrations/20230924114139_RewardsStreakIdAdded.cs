using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotApi.Migrations
{
    public partial class RewardsStreakIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StreakId",
                table: "DailyRewards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreakId",
                table: "DailyRewards");
        }
    }
}

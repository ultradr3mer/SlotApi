using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotApi.Migrations
{
    /// <inheritdoc />
    public partial class SpinsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SlotSpins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResultJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscordUserDiscordId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotSpins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotSpins_User_DiscordUserDiscordId",
                        column: x => x.DiscordUserDiscordId,
                        principalTable: "User",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SlotSpins_DiscordUserDiscordId",
                table: "SlotSpins",
                column: "DiscordUserDiscordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SlotSpins");
        }
    }
}

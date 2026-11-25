using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RollForBears.Modules.RpgContent.Migrations
{
    /// <inheritdoc />
    public partial class CreateGameSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rpg_content");

            migrationBuilder.CreateTable(
                name: "game_system",
                schema: "rpg_content",
                columns: table => new
                {
                    g_system_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    system_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    genre_set = table.Column<string[]>(type: "text[]", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_system", x => x.g_system_uuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_system_system_key",
                schema: "rpg_content",
                table: "game_system",
                column: "system_key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_system",
                schema: "rpg_content");
        }
    }
}

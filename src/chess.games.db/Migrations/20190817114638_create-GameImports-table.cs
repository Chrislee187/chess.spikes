using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class createGameImportstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventId = table.Column<Guid>(nullable: true),
                    SiteId = table.Column<Guid>(nullable: true),
                    WhiteId = table.Column<Guid>(nullable: true),
                    BlackId = table.Column<Guid>(nullable: true),
                    Date = table.Column<string>(maxLength: 30, nullable: true),
                    Round = table.Column<string>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    MoveText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameImports_Players_BlackId",
                        column: x => x.BlackId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameImports_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameImports_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameImports_Players_WhiteId",
                        column: x => x.WhiteId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            migrationBuilder.CreateIndex(
                name: "IX_GameImports_BlackId",
                table: "GameImports",
                column: "BlackId");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_EventId",
                table: "GameImports",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_SiteId",
                table: "GameImports",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_WhiteId",
                table: "GameImports",
                column: "WhiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameImports");
        }
    }
}

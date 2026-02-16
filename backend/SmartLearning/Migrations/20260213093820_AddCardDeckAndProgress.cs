using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddCardDeckAndProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    OwnerUserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    IsPublished = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SourceDeckId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decks_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Decks_Decks_SourceDeckId",
                        column: x => x.SourceDeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    DeckId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Front = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    Back = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserCardProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    CardId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StrategyType = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    StrategyDataJson = table.Column<string>(type: "json", maxLength: 2000, nullable: false),
                    LastReviewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NextReviewAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCardProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCardProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCardProgresses_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckId",
                table: "Cards",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_OwnerUserId",
                table: "Decks",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_SourceDeckId",
                table: "Decks",
                column: "SourceDeckId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCardProgresses_CardId",
                table: "UserCardProgresses",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCardProgresses_UserId",
                table: "UserCardProgresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCardProgresses");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Decks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");
        }
    }
}

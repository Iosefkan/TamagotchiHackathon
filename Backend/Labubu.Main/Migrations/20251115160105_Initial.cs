using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labubu.Main.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BattlePasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MaxLevel = table.Column<int>(type: "integer", nullable: false),
                    XpPerLevel = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattlePasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clothes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clothes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BattlePassRewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BattlePassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    ClothesId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrencyAmount = table.Column<int>(type: "integer", nullable: true),
                    CurrencyType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattlePassRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattlePassRewards_BattlePasses_BattlePassId",
                        column: x => x.BattlePassId,
                        principalTable: "BattlePasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattlePassRewards_Clothes_ClothesId",
                        column: x => x.ClothesId,
                        principalTable: "Clothes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Labubus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Energy = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    IsMessageRead = table.Column<bool>(type: "boolean", nullable: false),
                    LastMessageGeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labubus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labubus_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AchievementProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabubuId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AchievementProgresses_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AchievementProgresses_Labubus_LabubuId",
                        column: x => x.LabubuId,
                        principalTable: "Labubus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattlePassProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BattlePassId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabubuId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                    CurrentXp = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattlePassProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattlePassProgresses_BattlePasses_BattlePassId",
                        column: x => x.BattlePassId,
                        principalTable: "BattlePasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattlePassProgresses_Labubus_LabubuId",
                        column: x => x.LabubuId,
                        principalTable: "Labubus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattlePassPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BattlePassId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabubuId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PricePaid = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattlePassPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattlePassPurchases_BattlePasses_BattlePassId",
                        column: x => x.BattlePassId,
                        principalTable: "BattlePasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattlePassPurchases_Labubus_LabubuId",
                        column: x => x.LabubuId,
                        principalTable: "Labubus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattlePassRewardClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BattlePassRewardId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabubuId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattlePassRewardClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattlePassRewardClaims_BattlePassRewards_BattlePassRewardId",
                        column: x => x.BattlePassRewardId,
                        principalTable: "BattlePassRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattlePassRewardClaims_Labubus_LabubuId",
                        column: x => x.LabubuId,
                        principalTable: "Labubus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabubuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    SpecificData = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Award = table.Column<int>(type: "integer", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSessions_Labubus_LabubuId",
                        column: x => x.LabubuId,
                        principalTable: "Labubus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabubuClothes",
                columns: table => new
                {
                    LabubuId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClothesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabubuClothes", x => new { x.LabubuId, x.ClothesId });
                    table.ForeignKey(
                        name: "FK_LabubuClothes_Clothes_ClothesId",
                        column: x => x.ClothesId,
                        principalTable: "Clothes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabubuClothes_Labubus_LabubuId",
                        column: x => x.LabubuId,
                        principalTable: "Labubus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementProgresses_AchievementId",
                table: "AchievementProgresses",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementProgresses_LabubuId",
                table: "AchievementProgresses",
                column: "LabubuId");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassProgresses_BattlePassId_LabubuId",
                table: "BattlePassProgresses",
                columns: new[] { "BattlePassId", "LabubuId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassProgresses_LabubuId",
                table: "BattlePassProgresses",
                column: "LabubuId");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassPurchases_BattlePassId_LabubuId",
                table: "BattlePassPurchases",
                columns: new[] { "BattlePassId", "LabubuId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassPurchases_LabubuId",
                table: "BattlePassPurchases",
                column: "LabubuId");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassRewardClaims_BattlePassRewardId_LabubuId",
                table: "BattlePassRewardClaims",
                columns: new[] { "BattlePassRewardId", "LabubuId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassRewardClaims_LabubuId",
                table: "BattlePassRewardClaims",
                column: "LabubuId");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassRewards_BattlePassId",
                table: "BattlePassRewards",
                column: "BattlePassId");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePassRewards_ClothesId",
                table: "BattlePassRewards",
                column: "ClothesId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameId",
                table: "GameSessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_LabubuId",
                table: "GameSessions",
                column: "LabubuId");

            migrationBuilder.CreateIndex(
                name: "IX_LabubuClothes_ClothesId",
                table: "LabubuClothes",
                column: "ClothesId");

            migrationBuilder.CreateIndex(
                name: "IX_Labubus_UserId",
                table: "Labubus",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementProgresses");

            migrationBuilder.DropTable(
                name: "BattlePassProgresses");

            migrationBuilder.DropTable(
                name: "BattlePassPurchases");

            migrationBuilder.DropTable(
                name: "BattlePassRewardClaims");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "LabubuClothes");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "BattlePassRewards");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Labubus");

            migrationBuilder.DropTable(
                name: "BattlePasses");

            migrationBuilder.DropTable(
                name: "Clothes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

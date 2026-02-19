using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicesToUserCardProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserCardProgresses_UserId_CardId",
                table: "UserCardProgresses",
                columns: new[] { "UserId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCardProgresses_UserId_NextReviewAt",
                table: "UserCardProgresses",
                columns: new[] { "UserId", "NextReviewAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCardProgresses_UserId_CardId",
                table: "UserCardProgresses");

            migrationBuilder.DropIndex(
                name: "IX_UserCardProgresses_UserId_NextReviewAt",
                table: "UserCardProgresses");
            
        }
    }
}

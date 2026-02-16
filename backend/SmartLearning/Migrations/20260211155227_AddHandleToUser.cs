using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddHandleToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Xp",
                table: "AspNetUsers",
                newName: "Handle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Handle",
                table: "AspNetUsers",
                newName: "Xp");
        }
    }
}

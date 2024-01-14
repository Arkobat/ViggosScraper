using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViggosScraper.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRealName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RealName",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealName",
                table: "Users");
        }
    }
}

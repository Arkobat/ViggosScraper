using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViggosScraper.Database.Migrations
{
    /// <inheritdoc />
    public partial class PrivateDato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "LogosDates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Private",
                table: "LogosDates");
        }
    }
}

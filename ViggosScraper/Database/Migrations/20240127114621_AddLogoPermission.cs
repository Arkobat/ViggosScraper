using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViggosScraper.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Private",
                table: "LogosDates");

            migrationBuilder.AddColumn<string>(
                name: "Permission",
                table: "LogosDates",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permission",
                table: "LogosDates");

            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "LogosDates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

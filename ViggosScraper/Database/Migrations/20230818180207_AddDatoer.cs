using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ViggosScraper.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDatoer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogoGroups_Symbol",
                table: "LogoGroups");

            migrationBuilder.CreateTable(
                name: "DbDato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbDato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbDato_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfileId",
                table: "Users",
                column: "ProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogoGroups_Symbol",
                table: "LogoGroups",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DbDato_UserId",
                table: "DbDato",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbDato");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_LogoGroups_Symbol",
                table: "LogoGroups");

            migrationBuilder.CreateIndex(
                name: "IX_LogoGroups_Symbol",
                table: "LogoGroups",
                column: "Symbol");
        }
    }
}

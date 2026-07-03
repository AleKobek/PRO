using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Usuniecie_wydawcy_wspieranej_gry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wydawca",
                table: "Wspierana_gra");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "wydawca",
                table: "Wspierana_gra",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}

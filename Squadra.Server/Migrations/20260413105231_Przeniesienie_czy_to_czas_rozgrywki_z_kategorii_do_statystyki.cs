using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Przeniesienie_czy_to_czas_rozgrywki_z_kategorii_do_statystyki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "czy_to_czas_rozgrywki",
                table: "Kategoria");

            migrationBuilder.AddColumn<bool>(
                name: "czy_to_czas_rozgrywki",
                table: "Statystyka",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "czy_to_czas_rozgrywki",
                table: "Statystyka");

            migrationBuilder.AddColumn<bool>(
                name: "czy_to_czas_rozgrywki",
                table: "Kategoria",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

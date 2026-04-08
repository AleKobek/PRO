using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_kolumny_porownywalna_wartosc_liczbowa_do_statystyki_uzytkownika : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PorownywalnaWartoscLiczbowa",
                table: "StatystykaUzytkownika",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PorownywalnaWartoscLiczbowa",
                table: "StatystykaUzytkownika");
        }
    }
}

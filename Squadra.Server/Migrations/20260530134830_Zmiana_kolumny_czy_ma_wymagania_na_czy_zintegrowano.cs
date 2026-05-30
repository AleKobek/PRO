using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_kolumny_czy_ma_wymagania_na_czy_zintegrowano : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "czy_ma_wymagania",
                table: "Druzyna",
                newName: "czy_zintegrowano");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "czy_zintegrowano",
                table: "Druzyna",
                newName: "czy_ma_wymagania");
        }
    }
}

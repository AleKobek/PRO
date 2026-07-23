using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_kolumn_w_tabeli_uzytkownik_na_bardziej_bazodanowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OstatniaAktywnosc",
                table: "AspNetUsers",
                newName: "ostatnia_aktywnosc");

            migrationBuilder.RenameColumn(
                name: "DataUrodzenia",
                table: "AspNetUsers",
                newName: "data_urodzenia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ostatnia_aktywnosc",
                table: "AspNetUsers",
                newName: "OstatniaAktywnosc");

            migrationBuilder.RenameColumn(
                name: "data_urodzenia",
                table: "AspNetUsers",
                newName: "DataUrodzenia");
        }
    }
}

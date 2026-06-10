using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_czesci_kolumn_powiadomienia_na_bardziej_bazodanowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UzytkownikId",
                table: "Powiadomienie",
                newName: "id_uzytkownika");

            migrationBuilder.RenameColumn(
                name: "TypPowiadomieniaId",
                table: "Powiadomienie",
                newName: "id_typu_powiadomienia");

            migrationBuilder.RenameColumn(
                name: "PowiazanyObiektId",
                table: "Powiadomienie",
                newName: "id_powiazanego_obiektu");

            migrationBuilder.RenameIndex(
                name: "IX_Powiadomienie_UzytkownikId",
                table: "Powiadomienie",
                newName: "IX_Powiadomienie_id_uzytkownika");

            migrationBuilder.RenameIndex(
                name: "IX_Powiadomienie_TypPowiadomieniaId",
                table: "Powiadomienie",
                newName: "IX_Powiadomienie_id_typu_powiadomienia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "Powiadomienie",
                newName: "UzytkownikId");

            migrationBuilder.RenameColumn(
                name: "id_typu_powiadomienia",
                table: "Powiadomienie",
                newName: "TypPowiadomieniaId");

            migrationBuilder.RenameColumn(
                name: "id_powiazanego_obiektu",
                table: "Powiadomienie",
                newName: "PowiazanyObiektId");

            migrationBuilder.RenameIndex(
                name: "IX_Powiadomienie_id_uzytkownika",
                table: "Powiadomienie",
                newName: "IX_Powiadomienie_UzytkownikId");

            migrationBuilder.RenameIndex(
                name: "IX_Powiadomienie_id_typu_powiadomienia",
                table: "Powiadomienie",
                newName: "IX_Powiadomienie_TypPowiadomieniaId");
        }
    }
}

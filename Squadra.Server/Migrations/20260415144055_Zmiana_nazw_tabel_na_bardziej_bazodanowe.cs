using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_tabel_na_bardziej_bazodanowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategoria_WspieranaGra_id_gry",
                table: "Kategoria");

            migrationBuilder.DropForeignKey(
                name: "FK_Rola_WspieranaGra_IdGry",
                table: "Rola");

            migrationBuilder.DropForeignKey(
                name: "GraUzytkownika_Gra",
                table: "GraUzytkownika");

            migrationBuilder.DropForeignKey(
                name: "GraNaPlatformie_WspieranaGra",
                table: "GraNaPlatformie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WspieranaGra",
                table: "WspieranaGra");

            migrationBuilder.RenameTable(name: "WspieranaGra", newName: "Wspierana_gra");
            migrationBuilder.RenameTable(name: "UzytkownikPlatforma", newName: "Uzytkownik_platforma");
            migrationBuilder.RenameTable(name: "TypWiadomosci", newName: "Typ_wiadomosci");
            migrationBuilder.RenameTable(name: "TypPowiadomienia", newName: "Typ_powiadomienia");
            migrationBuilder.RenameTable(name: "StopienBieglosciJezyka", newName: "Stopien_bieglosci_jezyka");
            migrationBuilder.RenameTable(name: "StatystykaUzytkownika", newName: "Statystyka_uzytkownika");
            migrationBuilder.RenameTable(name: "JezykProfilu", newName: "Jezyk_profilu");
            migrationBuilder.RenameTable(name: "GraUzytkownikaNaPlatformie", newName: "Gra_uzytkownika_na_platformie");
            migrationBuilder.RenameTable(name: "GraUzytkownika", newName: "Gra_uzytkownika");
            migrationBuilder.RenameTable(name: "GraNaPlatformie", newName: "Gra_na_platformie");

            migrationBuilder.RenameIndex(name: "IX_UzytkownikPlatforma_id_uzytkownika", table: "Uzytkownik_platforma", newName: "IX_Uzytkownik_platforma_id_uzytkownika");
            migrationBuilder.RenameIndex(name: "IX_StatystykaUzytkownika_id_statystyki", table: "Statystyka_uzytkownika", newName: "IX_Statystyka_uzytkownika_id_statystyki");
            migrationBuilder.RenameIndex(name: "IX_JezykProfilu_id_uzytkownika", table: "Jezyk_profilu", newName: "IX_Jezyk_profilu_id_uzytkownika");
            migrationBuilder.RenameIndex(name: "IX_JezykProfilu_id_stopnia_bieglosci", table: "Jezyk_profilu", newName: "IX_Jezyk_profilu_id_stopnia_bieglosci");
            migrationBuilder.RenameIndex(name: "IX_GraUzytkownikaNaPlatformie_id_platformy", table: "Gra_uzytkownika_na_platformie", newName: "IX_Gra_uzytkownika_na_platformie_id_platformy");
            migrationBuilder.RenameIndex(name: "IX_GraUzytkownika_id_gry", table: "Gra_uzytkownika", newName: "IX_Gra_uzytkownika_id_gry");
            migrationBuilder.RenameIndex(name: "IX_GraNaPlatformie_id_platformy", table: "Gra_na_platformie", newName: "IX_Gra_na_platformie_id_platformy");

            migrationBuilder.AddPrimaryKey(name: "PK_Wspierana_gra", table: "Wspierana_gra", column: "id");

            migrationBuilder.AddForeignKey(name: "FK_Kategoria_Wspierana_gra_id_gry", table: "Kategoria", column: "id_gry", principalTable: "Wspierana_gra", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_Rola_Wspierana_gra_IdGry", table: "Rola", column: "IdGry", principalTable: "Wspierana_gra", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "GraUzytkownika_Gra", table: "Gra_uzytkownika", column: "id_gry", principalTable: "Wspierana_gra", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "GraNaPlatformie_WspieranaGra", table: "Gra_na_platformie", column: "id_gry", principalTable: "Wspierana_gra", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Kategoria_Wspierana_gra_id_gry", table: "Kategoria");
            migrationBuilder.DropForeignKey(name: "FK_Rola_Wspierana_gra_IdGry", table: "Rola");
            migrationBuilder.DropForeignKey(name: "GraUzytkownika_Gra", table: "Gra_uzytkownika");
            migrationBuilder.DropForeignKey(name: "GraNaPlatformie_WspieranaGra", table: "Gra_na_platformie");

            migrationBuilder.DropPrimaryKey(name: "PK_Wspierana_gra", table: "Wspierana_gra");

            migrationBuilder.RenameTable(name: "Wspierana_gra", newName: "WspieranaGra");
            migrationBuilder.RenameTable(name: "Uzytkownik_platforma", newName: "UzytkownikPlatforma");
            migrationBuilder.RenameTable(name: "Typ_wiadomosci", newName: "TypWiadomosci");
            migrationBuilder.RenameTable(name: "Typ_powiadomienia", newName: "TypPowiadomienia");
            migrationBuilder.RenameTable(name: "Stopien_bieglosci_jezyka", newName: "StopienBieglosciJezyka");
            migrationBuilder.RenameTable(name: "Statystyka_uzytkownika", newName: "StatystykaUzytkownika");
            migrationBuilder.RenameTable(name: "Jezyk_profilu", newName: "JezykProfilu");
            migrationBuilder.RenameTable(name: "Gra_uzytkownika_na_platformie", newName: "GraUzytkownikaNaPlatformie");
            migrationBuilder.RenameTable(name: "Gra_uzytkownika", newName: "GraUzytkownika");
            migrationBuilder.RenameTable(name: "Gra_na_platformie", newName: "GraNaPlatformie");

            migrationBuilder.RenameIndex(name: "IX_Uzytkownik_platforma_id_uzytkownika", table: "UzytkownikPlatforma", newName: "IX_UzytkownikPlatforma_id_uzytkownika");
            migrationBuilder.RenameIndex(name: "IX_Statystyka_uzytkownika_id_statystyki", table: "StatystykaUzytkownika", newName: "IX_StatystykaUzytkownika_id_statystyki");
            migrationBuilder.RenameIndex(name: "IX_Jezyk_profilu_id_uzytkownika", table: "JezykProfilu", newName: "IX_JezykProfilu_id_uzytkownika");
            migrationBuilder.RenameIndex(name: "IX_Jezyk_profilu_id_stopnia_bieglosci", table: "JezykProfilu", newName: "IX_JezykProfilu_id_stopnia_bieglosci");
            migrationBuilder.RenameIndex(name: "IX_Gra_uzytkownika_na_platformie_id_platformy", table: "GraUzytkownikaNaPlatformie", newName: "IX_GraUzytkownikaNaPlatformie_id_platformy");
            migrationBuilder.RenameIndex(name: "IX_Gra_uzytkownika_id_gry", table: "GraUzytkownika", newName: "IX_GraUzytkownika_id_gry");
            migrationBuilder.RenameIndex(name: "IX_Gra_na_platformie_id_platformy", table: "GraNaPlatformie", newName: "IX_GraNaPlatformie_id_platformy");

            migrationBuilder.AddPrimaryKey(name: "PK_WspieranaGra", table: "WspieranaGra", column: "id");

            migrationBuilder.AddForeignKey(name: "FK_Kategoria_WspieranaGra_id_gry", table: "Kategoria", column: "id_gry", principalTable: "WspieranaGra", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_Rola_WspieranaGra_IdGry", table: "Rola", column: "IdGry", principalTable: "WspieranaGra", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "GraUzytkownika_Gra", table: "GraUzytkownika", column: "GraId", principalTable: "WspieranaGra", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "GraNaPlatformie_WspieranaGra", table: "GraNaPlatformie", column: "IdWspieranejGry", principalTable: "WspieranaGra", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        }
    }
}

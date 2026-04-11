using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_kolumn_na_bardziej_bazodanowe_oraz_usuniecie_pozostalosci_nazwy_jezyk_uzytkownika_oraz_tymczasowe_usuniecie_pk_kraju : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "JezykUzytkownika_StopienBieglosci",
                table: "JezykProfilu");

            migrationBuilder.DropForeignKey(
                name: "FK_Kategoria_WspieranaGra_IdGry",
                table: "Kategoria");

            migrationBuilder.DropForeignKey(
                name: "FK_Profil_AspNetUsers_IdUzytkownika",
                table: "Profil");

            migrationBuilder.DropForeignKey(
                name: "Region_Kraj",
                table: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "id",
                table: "Kraj");

            migrationBuilder.DropPrimaryKey(
                name: "id_jezyk_uzytkownika",
                table: "JezykProfilu");

            migrationBuilder.RenameColumn(
                name: "DataNawiazaniaZnajomosci",
                table: "Znajomi",
                newName: "data_nawiazania_znajomosci");

            migrationBuilder.RenameColumn(
                name: "IdUzytkownika2",
                table: "Znajomi",
                newName: "id_uzytkownika_2");

            migrationBuilder.RenameColumn(
                name: "IdUzytkownika1",
                table: "Znajomi",
                newName: "id_uzytkownika_1");

            migrationBuilder.RenameIndex(
                name: "IX_Znajomi_IdUzytkownika2",
                table: "Znajomi",
                newName: "IX_Znajomi_id_uzytkownika_2");

            migrationBuilder.RenameColumn(
                name: "Wydawca",
                table: "WspieranaGra",
                newName: "wydawca");

            migrationBuilder.RenameColumn(
                name: "Tytul",
                table: "WspieranaGra",
                newName: "tytul");

            migrationBuilder.RenameColumn(
                name: "Gatunek",
                table: "WspieranaGra",
                newName: "gatunek");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WspieranaGra",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Tresc",
                table: "Wiadomosc",
                newName: "tresc");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Wiadomosc",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DataWyslania",
                table: "Wiadomosc",
                newName: "data_wyslania");

            migrationBuilder.RenameColumn(
                name: "PseudonimNaPlatformie",
                table: "UzytkownikPlatforma",
                newName: "pseudonim_na_platformie");

            migrationBuilder.RenameColumn(
                name: "UzytkownikId",
                table: "UzytkownikPlatforma",
                newName: "id_uzytkownika");

            migrationBuilder.RenameColumn(
                name: "PlatformaId",
                table: "UzytkownikPlatforma",
                newName: "id_platformy");

            migrationBuilder.RenameIndex(
                name: "IX_UzytkownikPlatforma_UzytkownikId",
                table: "UzytkownikPlatforma",
                newName: "IX_UzytkownikPlatforma_id_uzytkownika");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "TypWiadomosci",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TypWiadomosci",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "TypPowiadomienia",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TypPowiadomienia",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Wartosc",
                table: "StopienBieglosciJezyka",
                newName: "wartosc");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "StopienBieglosciJezyka",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StopienBieglosciJezyka",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Wartosc",
                table: "StatystykaUzytkownika",
                newName: "wartosc");

            migrationBuilder.RenameColumn(
                name: "PorownywalnaWartoscLiczbowa",
                table: "StatystykaUzytkownika",
                newName: "porownywalna_wartosc_liczbowa");

            migrationBuilder.RenameColumn(
                name: "StatystykaId",
                table: "StatystykaUzytkownika",
                newName: "id_statystyki");

            migrationBuilder.RenameColumn(
                name: "UzytkownikId",
                table: "StatystykaUzytkownika",
                newName: "id_uzytkownika");

            migrationBuilder.RenameIndex(
                name: "IX_StatystykaUzytkownika_StatystykaId",
                table: "StatystykaUzytkownika",
                newName: "IX_StatystykaUzytkownika_id_statystyki");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Statystyka",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Statystyka",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RolaId",
                table: "Statystyka",
                newName: "id_roli");

            migrationBuilder.RenameColumn(
                name: "KategoriaId",
                table: "Statystyka",
                newName: "id_kategorii");

            migrationBuilder.RenameIndex(
                name: "IX_Statystyka_RolaId",
                table: "Statystyka",
                newName: "IX_Statystyka_id_roli");

            migrationBuilder.RenameIndex(
                name: "IX_Statystyka_KategoriaId",
                table: "Statystyka",
                newName: "IX_Statystyka_id_kategorii");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Status",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Status",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Rola",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Rola",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Region",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Region",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Zaimki",
                table: "Profil",
                newName: "zaimki");

            migrationBuilder.RenameColumn(
                name: "Pseudonim",
                table: "Profil",
                newName: "pseudonim");

            migrationBuilder.RenameColumn(
                name: "Opis",
                table: "Profil",
                newName: "opis");

            migrationBuilder.RenameColumn(
                name: "IdUzytkownika",
                table: "Profil",
                newName: "id_uzytkownika");

            migrationBuilder.RenameColumn(
                name: "Tresc",
                table: "Powiadomienie",
                newName: "tresc");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Powiadomienie",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PowiazanyObiektNazwa",
                table: "Powiadomienie",
                newName: "nazwa_powiazanego_obiektu");

            migrationBuilder.RenameColumn(
                name: "DataWyslania",
                table: "Powiadomienie",
                newName: "data_wyslania");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Platforma",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "Platforma",
                newName: "logo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Platforma",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Kraj",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Kraj",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Kategoria",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Kategoria",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IdGry",
                table: "Kategoria",
                newName: "id_gry");

            migrationBuilder.RenameColumn(
                name: "CzyToCzasRozgrywki",
                table: "Kategoria",
                newName: "czy_to_czas_rozgrywki");

            migrationBuilder.RenameIndex(
                name: "IX_Kategoria_IdGry",
                table: "Kategoria",
                newName: "IX_Kategoria_id_gry");

            migrationBuilder.RenameColumn(
                name: "StopienBieglosciId",
                table: "JezykProfilu",
                newName: "id_stopnia_bieglosci");

            migrationBuilder.RenameColumn(
                name: "UzytkownikId",
                table: "JezykProfilu",
                newName: "id_uzytkownika");

            migrationBuilder.RenameColumn(
                name: "JezykId",
                table: "JezykProfilu",
                newName: "id_jezyka");

            migrationBuilder.RenameIndex(
                name: "IX_JezykProfilu_UzytkownikId",
                table: "JezykProfilu",
                newName: "IX_JezykProfilu_id_uzytkownika");

            migrationBuilder.RenameIndex(
                name: "IX_JezykProfilu_StopienBieglosciId",
                table: "JezykProfilu",
                newName: "IX_JezykProfilu_id_stopnia_bieglosci");

            migrationBuilder.RenameColumn(
                name: "Nazwa",
                table: "Jezyk",
                newName: "nazwa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Jezyk",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PlatformaId",
                table: "GraUzytkownikaNaPlatformie",
                newName: "id_platformy");

            migrationBuilder.RenameColumn(
                name: "GraId",
                table: "GraUzytkownikaNaPlatformie",
                newName: "id_gry");

            migrationBuilder.RenameColumn(
                name: "UzytkownikId",
                table: "GraUzytkownikaNaPlatformie",
                newName: "id_uzytkownika");

            migrationBuilder.RenameIndex(
                name: "IX_GraUzytkownikaNaPlatformie_PlatformaId",
                table: "GraUzytkownikaNaPlatformie",
                newName: "IX_GraUzytkownikaNaPlatformie_id_platformy");

            migrationBuilder.RenameColumn(
                name: "GraId",
                table: "GraUzytkownika",
                newName: "id_gry");

            migrationBuilder.RenameColumn(
                name: "UzytkownikId",
                table: "GraUzytkownika",
                newName: "id_uzytkownika");

            migrationBuilder.RenameIndex(
                name: "IX_GraUzytkownika_GraId",
                table: "GraUzytkownika",
                newName: "IX_GraUzytkownika_id_gry");

            migrationBuilder.RenameColumn(
                name: "IdPlatformy",
                table: "GraNaPlatformie",
                newName: "id_platformy");

            migrationBuilder.RenameColumn(
                name: "IdWspieranejGry",
                table: "GraNaPlatformie",
                newName: "id_gry");

            migrationBuilder.RenameIndex(
                name: "IX_GraNaPlatformie_IdPlatformy",
                table: "GraNaPlatformie",
                newName: "IX_GraNaPlatformie_id_platformy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kraj",
                table: "Kraj",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "id_jezyk_profilu",
                table: "JezykProfilu",
                columns: new[] { "id_jezyka", "id_uzytkownika" });

            migrationBuilder.AddForeignKey(
                name: "JezykProfilu_StopienBieglosci",
                table: "JezykProfilu",
                column: "id_stopnia_bieglosci",
                principalTable: "StopienBieglosciJezyka",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Kategoria_WspieranaGra_id_gry",
                table: "Kategoria",
                column: "id_gry",
                principalTable: "WspieranaGra",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profil_AspNetUsers_id_uzytkownika",
                table: "Profil",
                column: "id_uzytkownika",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Region_Kraj_KrajId",
                table: "Region",
                column: "KrajId",
                principalTable: "Kraj",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "JezykProfilu_StopienBieglosci",
                table: "JezykProfilu");

            migrationBuilder.DropForeignKey(
                name: "FK_Kategoria_WspieranaGra_id_gry",
                table: "Kategoria");

            migrationBuilder.DropForeignKey(
                name: "FK_Profil_AspNetUsers_id_uzytkownika",
                table: "Profil");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_Kraj_KrajId",
                table: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kraj",
                table: "Kraj");

            migrationBuilder.DropPrimaryKey(
                name: "id_jezyk_profilu",
                table: "JezykProfilu");

            migrationBuilder.RenameColumn(
                name: "data_nawiazania_znajomosci",
                table: "Znajomi",
                newName: "DataNawiazaniaZnajomosci");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika_2",
                table: "Znajomi",
                newName: "IdUzytkownika2");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika_1",
                table: "Znajomi",
                newName: "IdUzytkownika1");

            migrationBuilder.RenameIndex(
                name: "IX_Znajomi_id_uzytkownika_2",
                table: "Znajomi",
                newName: "IX_Znajomi_IdUzytkownika2");

            migrationBuilder.RenameColumn(
                name: "wydawca",
                table: "WspieranaGra",
                newName: "Wydawca");

            migrationBuilder.RenameColumn(
                name: "tytul",
                table: "WspieranaGra",
                newName: "Tytul");

            migrationBuilder.RenameColumn(
                name: "gatunek",
                table: "WspieranaGra",
                newName: "Gatunek");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "WspieranaGra",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tresc",
                table: "Wiadomosc",
                newName: "Tresc");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Wiadomosc",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "data_wyslania",
                table: "Wiadomosc",
                newName: "DataWyslania");

            migrationBuilder.RenameColumn(
                name: "pseudonim_na_platformie",
                table: "UzytkownikPlatforma",
                newName: "PseudonimNaPlatformie");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "UzytkownikPlatforma",
                newName: "UzytkownikId");

            migrationBuilder.RenameColumn(
                name: "id_platformy",
                table: "UzytkownikPlatforma",
                newName: "PlatformaId");

            migrationBuilder.RenameIndex(
                name: "IX_UzytkownikPlatforma_id_uzytkownika",
                table: "UzytkownikPlatforma",
                newName: "IX_UzytkownikPlatforma_UzytkownikId");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "TypWiadomosci",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TypWiadomosci",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "TypPowiadomienia",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TypPowiadomienia",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "wartosc",
                table: "StopienBieglosciJezyka",
                newName: "Wartosc");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "StopienBieglosciJezyka",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "StopienBieglosciJezyka",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "wartosc",
                table: "StatystykaUzytkownika",
                newName: "Wartosc");

            migrationBuilder.RenameColumn(
                name: "porownywalna_wartosc_liczbowa",
                table: "StatystykaUzytkownika",
                newName: "PorownywalnaWartoscLiczbowa");

            migrationBuilder.RenameColumn(
                name: "id_statystyki",
                table: "StatystykaUzytkownika",
                newName: "StatystykaId");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "StatystykaUzytkownika",
                newName: "UzytkownikId");

            migrationBuilder.RenameIndex(
                name: "IX_StatystykaUzytkownika_id_statystyki",
                table: "StatystykaUzytkownika",
                newName: "IX_StatystykaUzytkownika_StatystykaId");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Statystyka",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Statystyka",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id_roli",
                table: "Statystyka",
                newName: "RolaId");

            migrationBuilder.RenameColumn(
                name: "id_kategorii",
                table: "Statystyka",
                newName: "KategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Statystyka_id_roli",
                table: "Statystyka",
                newName: "IX_Statystyka_RolaId");

            migrationBuilder.RenameIndex(
                name: "IX_Statystyka_id_kategorii",
                table: "Statystyka",
                newName: "IX_Statystyka_KategoriaId");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Status",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Status",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Rola",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Rola",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Region",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Region",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "zaimki",
                table: "Profil",
                newName: "Zaimki");

            migrationBuilder.RenameColumn(
                name: "pseudonim",
                table: "Profil",
                newName: "Pseudonim");

            migrationBuilder.RenameColumn(
                name: "opis",
                table: "Profil",
                newName: "Opis");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "Profil",
                newName: "IdUzytkownika");

            migrationBuilder.RenameColumn(
                name: "tresc",
                table: "Powiadomienie",
                newName: "Tresc");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Powiadomienie",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nazwa_powiazanego_obiektu",
                table: "Powiadomienie",
                newName: "PowiazanyObiektNazwa");

            migrationBuilder.RenameColumn(
                name: "data_wyslania",
                table: "Powiadomienie",
                newName: "DataWyslania");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Platforma",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "logo",
                table: "Platforma",
                newName: "Logo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Platforma",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Kraj",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Kraj",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Kategoria",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Kategoria",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id_gry",
                table: "Kategoria",
                newName: "IdGry");

            migrationBuilder.RenameColumn(
                name: "czy_to_czas_rozgrywki",
                table: "Kategoria",
                newName: "CzyToCzasRozgrywki");

            migrationBuilder.RenameIndex(
                name: "IX_Kategoria_id_gry",
                table: "Kategoria",
                newName: "IX_Kategoria_IdGry");

            migrationBuilder.RenameColumn(
                name: "id_stopnia_bieglosci",
                table: "JezykProfilu",
                newName: "StopienBieglosciId");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "JezykProfilu",
                newName: "UzytkownikId");

            migrationBuilder.RenameColumn(
                name: "id_jezyka",
                table: "JezykProfilu",
                newName: "JezykId");

            migrationBuilder.RenameIndex(
                name: "IX_JezykProfilu_id_uzytkownika",
                table: "JezykProfilu",
                newName: "IX_JezykProfilu_UzytkownikId");

            migrationBuilder.RenameIndex(
                name: "IX_JezykProfilu_id_stopnia_bieglosci",
                table: "JezykProfilu",
                newName: "IX_JezykProfilu_StopienBieglosciId");

            migrationBuilder.RenameColumn(
                name: "nazwa",
                table: "Jezyk",
                newName: "Nazwa");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Jezyk",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id_platformy",
                table: "GraUzytkownikaNaPlatformie",
                newName: "PlatformaId");

            migrationBuilder.RenameColumn(
                name: "id_gry",
                table: "GraUzytkownikaNaPlatformie",
                newName: "GraId");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "GraUzytkownikaNaPlatformie",
                newName: "UzytkownikId");

            migrationBuilder.RenameIndex(
                name: "IX_GraUzytkownikaNaPlatformie_id_platformy",
                table: "GraUzytkownikaNaPlatformie",
                newName: "IX_GraUzytkownikaNaPlatformie_PlatformaId");

            migrationBuilder.RenameColumn(
                name: "id_gry",
                table: "GraUzytkownika",
                newName: "GraId");

            migrationBuilder.RenameColumn(
                name: "id_uzytkownika",
                table: "GraUzytkownika",
                newName: "UzytkownikId");

            migrationBuilder.RenameIndex(
                name: "IX_GraUzytkownika_id_gry",
                table: "GraUzytkownika",
                newName: "IX_GraUzytkownika_GraId");

            migrationBuilder.RenameColumn(
                name: "id_platformy",
                table: "GraNaPlatformie",
                newName: "IdPlatformy");

            migrationBuilder.RenameColumn(
                name: "id_gry",
                table: "GraNaPlatformie",
                newName: "IdWspieranejGry");

            migrationBuilder.RenameIndex(
                name: "IX_GraNaPlatformie_id_platformy",
                table: "GraNaPlatformie",
                newName: "IX_GraNaPlatformie_IdPlatformy");

            migrationBuilder.AddPrimaryKey(
                name: "id",
                table: "Kraj",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "id_jezyk_uzytkownika",
                table: "JezykProfilu",
                columns: new[] { "JezykId", "UzytkownikId" });

            migrationBuilder.AddForeignKey(
                name: "JezykUzytkownika_StopienBieglosci",
                table: "JezykProfilu",
                column: "StopienBieglosciId",
                principalTable: "StopienBieglosciJezyka",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Kategoria_WspieranaGra_IdGry",
                table: "Kategoria",
                column: "IdGry",
                principalTable: "WspieranaGra",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profil_AspNetUsers_IdUzytkownika",
                table: "Profil",
                column: "IdUzytkownika",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Region_Kraj",
                table: "Region",
                column: "KrajId",
                principalTable: "Kraj",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

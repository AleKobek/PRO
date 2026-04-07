using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_typu_kolumny_wartosc_tabeli_StatystykaUzytkownika_na_string_oraz_chwilowa_zmiana_nazw_kolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_znajomego_2",
                table: "Znajomi",
                newName: "IdUzytkownika2");

            migrationBuilder.RenameColumn(
                name: "id_znajomego_1",
                table: "Znajomi",
                newName: "IdUzytkownika1");

            migrationBuilder.RenameIndex(
                name: "IX_Znajomi_id_znajomego_2",
                table: "Znajomi",
                newName: "IX_Znajomi_IdUzytkownika2");

            migrationBuilder.RenameColumn(
                name: "id_odbiorcy",
                table: "Wiadomosc",
                newName: "IdOdbiorcy");

            migrationBuilder.RenameColumn(
                name: "id_nadawcy",
                table: "Wiadomosc",
                newName: "IdNadawcy");

            migrationBuilder.RenameIndex(
                name: "IX_Wiadomosc_id_odbiorcy",
                table: "Wiadomosc",
                newName: "IX_Wiadomosc_IdOdbiorcy");

            migrationBuilder.RenameIndex(
                name: "IX_Wiadomosc_id_nadawcy",
                table: "Wiadomosc",
                newName: "IX_Wiadomosc_IdNadawcy");

            migrationBuilder.RenameColumn(
                name: "id_kraju",
                table: "Region",
                newName: "KrajId");

            migrationBuilder.RenameIndex(
                name: "IX_Region_id_kraju",
                table: "Region",
                newName: "IX_Region_KrajId");

            migrationBuilder.RenameColumn(
                name: "id_statusu",
                table: "Profil",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "id_regionu",
                table: "Profil",
                newName: "RegionId");

            migrationBuilder.RenameIndex(
                name: "IX_Profil_id_statusu",
                table: "Profil",
                newName: "IX_Profil_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Profil_id_regionu",
                table: "Profil",
                newName: "IX_Profil_RegionId");

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

            migrationBuilder.AlterColumn<string>(
                name: "Wartosc",
                table: "StatystykaUzytkownika",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdUzytkownika2",
                table: "Znajomi",
                newName: "id_znajomego_2");

            migrationBuilder.RenameColumn(
                name: "IdUzytkownika1",
                table: "Znajomi",
                newName: "id_znajomego_1");

            migrationBuilder.RenameIndex(
                name: "IX_Znajomi_IdUzytkownika2",
                table: "Znajomi",
                newName: "IX_Znajomi_id_znajomego_2");

            migrationBuilder.RenameColumn(
                name: "IdOdbiorcy",
                table: "Wiadomosc",
                newName: "id_odbiorcy");

            migrationBuilder.RenameColumn(
                name: "IdNadawcy",
                table: "Wiadomosc",
                newName: "id_nadawcy");

            migrationBuilder.RenameIndex(
                name: "IX_Wiadomosc_IdOdbiorcy",
                table: "Wiadomosc",
                newName: "IX_Wiadomosc_id_odbiorcy");

            migrationBuilder.RenameIndex(
                name: "IX_Wiadomosc_IdNadawcy",
                table: "Wiadomosc",
                newName: "IX_Wiadomosc_id_nadawcy");

            migrationBuilder.RenameColumn(
                name: "KrajId",
                table: "Region",
                newName: "id_kraju");

            migrationBuilder.RenameIndex(
                name: "IX_Region_KrajId",
                table: "Region",
                newName: "IX_Region_id_kraju");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Profil",
                newName: "id_statusu");

            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "Profil",
                newName: "id_regionu");

            migrationBuilder.RenameIndex(
                name: "IX_Profil_StatusId",
                table: "Profil",
                newName: "IX_Profil_id_statusu");

            migrationBuilder.RenameIndex(
                name: "IX_Profil_RegionId",
                table: "Profil",
                newName: "IX_Profil_id_regionu");

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

            migrationBuilder.AlterColumn<int>(
                name: "Wartosc",
                table: "StatystykaUzytkownika",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}

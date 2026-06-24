using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_kolumn_wymaganych_statystyk_druzyny_na_bardziej_bazodanowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatystykaId",
                table: "Wymagana_Statystyka_Druzyny",
                newName: "id_statystyki");

            migrationBuilder.RenameColumn(
                name: "DruzynaId",
                table: "Wymagana_Statystyka_Druzyny",
                newName: "id_druzyny");

            migrationBuilder.RenameIndex(
                name: "IX_Wymagana_Statystyka_Druzyny_StatystykaId",
                table: "Wymagana_Statystyka_Druzyny",
                newName: "IX_Wymagana_Statystyka_Druzyny_id_statystyki");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_statystyki",
                table: "Wymagana_Statystyka_Druzyny",
                newName: "StatystykaId");

            migrationBuilder.RenameColumn(
                name: "id_druzyny",
                table: "Wymagana_Statystyka_Druzyny",
                newName: "DruzynaId");

            migrationBuilder.RenameIndex(
                name: "IX_Wymagana_Statystyka_Druzyny_id_statystyki",
                table: "Wymagana_Statystyka_Druzyny",
                newName: "IX_Wymagana_Statystyka_Druzyny_StatystykaId");
        }
    }
}

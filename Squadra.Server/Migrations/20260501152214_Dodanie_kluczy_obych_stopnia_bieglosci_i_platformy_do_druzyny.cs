using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_kluczy_obych_stopnia_bieglosci_i_platformy_do_druzyny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Druzyna_Platforma_id_platformy",
                table: "Druzyna");

            migrationBuilder.DropForeignKey(
                name: "FK_Druzyna_Stopien_bieglosci_jezyka_id_wymaganego_stopnia_bieglosci_jezyka",
                table: "Druzyna");

            migrationBuilder.AddForeignKey(
                name: "Druzyna_Platforma",
                table: "Druzyna",
                column: "id_platformy",
                principalTable: "Platforma",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "Druzyna_Wymagany_Stopien_Bieglosci_Jezyka",
                table: "Druzyna",
                column: "id_wymaganego_stopnia_bieglosci_jezyka",
                principalTable: "Stopien_bieglosci_jezyka",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Druzyna_Platforma",
                table: "Druzyna");

            migrationBuilder.DropForeignKey(
                name: "Druzyna_Wymagany_Stopien_Bieglosci_Jezyka",
                table: "Druzyna");

            migrationBuilder.AddForeignKey(
                name: "FK_Druzyna_Platforma_id_platformy",
                table: "Druzyna",
                column: "id_platformy",
                principalTable: "Platforma",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Druzyna_Stopien_bieglosci_jezyka_id_wymaganego_stopnia_bieglosci_jezyka",
                table: "Druzyna",
                column: "id_wymaganego_stopnia_bieglosci_jezyka",
                principalTable: "Stopien_bieglosci_jezyka",
                principalColumn: "id");
        }
    }
}

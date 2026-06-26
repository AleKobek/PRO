using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_kolumn_id_nadawcy_i_odbiorcy_na_bardziej_bazodanowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdOdbiorcy",
                table: "Wiadomosc",
                newName: "id_odbiorcy");

            migrationBuilder.RenameColumn(
                name: "IdNadawcy",
                table: "Wiadomosc",
                newName: "id_nadawcy");

            migrationBuilder.RenameIndex(
                name: "IX_Wiadomosc_IdNadawcy",
                table: "Wiadomosc",
                newName: "IX_Wiadomosc_id_nadawcy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_odbiorcy",
                table: "Wiadomosc",
                newName: "IdOdbiorcy");

            migrationBuilder.RenameColumn(
                name: "id_nadawcy",
                table: "Wiadomosc",
                newName: "IdNadawcy");

            migrationBuilder.RenameIndex(
                name: "IX_Wiadomosc_id_nadawcy",
                table: "Wiadomosc",
                newName: "IX_Wiadomosc_IdNadawcy");
        }
    }
}

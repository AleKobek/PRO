using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Usuniecie_klucza_obcego_uzytkownika_jako_id_odbiorcy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Wiadomosc_Odbiorca",
                table: "Wiadomosc");

            migrationBuilder.DropIndex(
                name: "IX_Wiadomosc_IdOdbiorcy",
                table: "Wiadomosc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Wiadomosc_IdOdbiorcy",
                table: "Wiadomosc",
                column: "IdOdbiorcy");

            migrationBuilder.AddForeignKey(
                name: "Wiadomosc_Odbiorca",
                table: "Wiadomosc",
                column: "IdOdbiorcy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

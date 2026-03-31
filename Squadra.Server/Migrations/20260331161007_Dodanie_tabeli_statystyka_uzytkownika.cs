using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_tabeli_statystyka_uzytkownika : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatystykaUzytkownika",
                columns: table => new
                {
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    StatystykaId = table.Column<int>(type: "int", nullable: false),
                    Wartosc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_statystyka_uzytkownika", x => new { x.UzytkownikId, x.StatystykaId });
                    table.ForeignKey(
                        name: "StatystykaUzytkownika_Statystyka",
                        column: x => x.StatystykaId,
                        principalTable: "Statystyka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "StatystykaUzytkownika_Uzytkownik",
                        column: x => x.UzytkownikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatystykaUzytkownika_StatystykaId",
                table: "StatystykaUzytkownika",
                column: "StatystykaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatystykaUzytkownika");
        }
    }
}

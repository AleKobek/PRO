using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_wymaganej_statystyki_druzyny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wymagana_Statystyka_Druzyny",
                columns: table => new
                {
                    DruzynaId = table.Column<int>(type: "int", nullable: false),
                    StatystykaId = table.Column<int>(type: "int", nullable: false),
                    wartosc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    porownywalna_wartosc_liczbowa = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wymagana_Statystyka_Druzyny", x => new { x.DruzynaId, x.StatystykaId });
                    table.ForeignKey(
                        name: "WymaganaStatystykaDruzyny_Druzyna",
                        column: x => x.DruzynaId,
                        principalTable: "Druzyna",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Wymagana_Statystyka_Druzyny_Statystyka",
                        column: x => x.StatystykaId,
                        principalTable: "Statystyka",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wymagana_Statystyka_Druzyny_StatystykaId",
                table: "Wymagana_Statystyka_Druzyny",
                column: "StatystykaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wymagana_Statystyka_Druzyny");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_druzyn_i_nastroju_rozgrywki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nastroj_Rozgrywki",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazwa = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nastroj_Rozgrywki", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Druzyna",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazwa = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    id_gry = table.Column<int>(type: "int", nullable: false),
                    id_kapitana = table.Column<int>(type: "int", nullable: false),
                    czy_publiczna = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    opis = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    id_nastroju_rozgrywki = table.Column<int>(type: "int", nullable: true),
                    id_wymaganego_jezyka = table.Column<int>(type: "int", nullable: true),
                    id_wymaganego_stopnia_bieglosci_jezyka = table.Column<int>(type: "int", nullable: true),
                    czy_18_plus = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    id_platformy = table.Column<int>(type: "int", nullable: true),
                    czy_ma_wymagania = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Druzyna", x => x.id);
                    table.ForeignKey(
                        name: "Druzyna_Gra",
                        column: x => x.id_gry,
                        principalTable: "Wspierana_gra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Druzyna_Kapitan",
                        column: x => x.id_kapitana,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Druzyna_Nastroj_Rozgrywki",
                        column: x => x.id_nastroju_rozgrywki,
                        principalTable: "Nastroj_Rozgrywki",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Druzyna_Wymagany_Jezyk",
                        column: x => x.id_wymaganego_jezyka,
                        principalTable: "Jezyk",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Druzyna_Platforma_id_platformy",
                        column: x => x.id_platformy,
                        principalTable: "Platforma",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Druzyna_Stopien_bieglosci_jezyka_id_wymaganego_stopnia_bieglosci_jezyka",
                        column: x => x.id_wymaganego_stopnia_bieglosci_jezyka,
                        principalTable: "Stopien_bieglosci_jezyka",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Druzyna_id_gry",
                table: "Druzyna",
                column: "id_gry");

            migrationBuilder.CreateIndex(
                name: "IX_Druzyna_id_kapitana",
                table: "Druzyna",
                column: "id_kapitana");

            migrationBuilder.CreateIndex(
                name: "IX_Druzyna_id_nastroju_rozgrywki",
                table: "Druzyna",
                column: "id_nastroju_rozgrywki");

            migrationBuilder.CreateIndex(
                name: "IX_Druzyna_id_platformy",
                table: "Druzyna",
                column: "id_platformy");

            migrationBuilder.CreateIndex(
                name: "IX_Druzyna_id_wymaganego_jezyka",
                table: "Druzyna",
                column: "id_wymaganego_jezyka");

            migrationBuilder.CreateIndex(
                name: "IX_Druzyna_id_wymaganego_stopnia_bieglosci_jezyka",
                table: "Druzyna",
                column: "id_wymaganego_stopnia_bieglosci_jezyka");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Druzyna");

            migrationBuilder.DropTable(
                name: "Nastroj_Rozgrywki");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_tabeli_gra_uzytkownika_na_platformie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GraUzytkownikaNaPlatformie",
                columns: table => new
                {
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    GraId = table.Column<int>(type: "int", nullable: false),
                    PlatformaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_gra_uzytkownika_na_platformie", x => new { x.UzytkownikId, x.GraId, x.PlatformaId });
                    table.ForeignKey(
                        name: "GraUzytkownikaNaPlatformie_GraUzytkownika",
                        columns: x => new { x.UzytkownikId, x.GraId },
                        principalTable: "GraUzytkownika",
                        principalColumns: new[] { "UzytkownikId", "GraId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "GraUzytkownikaNaPlatformie_Platforma",
                        column: x => x.PlatformaId,
                        principalTable: "Platforma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraUzytkownikaNaPlatformie_PlatformaId",
                table: "GraUzytkownikaNaPlatformie",
                column: "PlatformaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraUzytkownikaNaPlatformie");
        }
    }
}

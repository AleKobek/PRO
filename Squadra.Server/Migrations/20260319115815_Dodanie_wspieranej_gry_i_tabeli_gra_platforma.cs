using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_wspieranej_gry_i_tabeli_gra_platforma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WspieranaGra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Tytul = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Wydawca = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Gatunek = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WspieranaGra", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraNaPlatformie",
                columns: table => new
                {
                    IdWspieranejGry = table.Column<int>(type: "int", nullable: false),
                    IdPlatformy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_gra_na_platformie", x => new { x.IdWspieranejGry, x.IdPlatformy });
                    table.ForeignKey(
                        name: "GraNaPlatformie_Platforma",
                        column: x => x.IdPlatformy,
                        principalTable: "Platforma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "GraNaPlatformie_WspieranaGra",
                        column: x => x.IdWspieranejGry,
                        principalTable: "WspieranaGra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraNaPlatformie_IdPlatformy",
                table: "GraNaPlatformie",
                column: "IdPlatformy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraNaPlatformie");

            migrationBuilder.DropTable(
                name: "WspieranaGra");
        }
    }
}

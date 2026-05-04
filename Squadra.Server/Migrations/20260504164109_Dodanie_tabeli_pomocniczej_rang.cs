using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_tabeli_pomocniczej_rang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ranga",
                columns: table => new
                {
                    id_statystyki = table.Column<int>(type: "int", nullable: false),
                    wartosc_liczbowa = table.Column<int>(type: "int", nullable: false),
                    nazwa = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranga", x => new { x.id_statystyki, x.wartosc_liczbowa });
                    table.ForeignKey(
                        name: "FK_Ranga_Statystyka_id_statystyki",
                        column: x => x.id_statystyki,
                        principalTable: "Statystyka",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ranga");
        }
    }
}

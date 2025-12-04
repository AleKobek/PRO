using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_znajomosci_oraz_typu_powiadomienia_usunieto_cie_ze_znajomych : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Znajomi",
                columns: table => new
                {
                    id_znajomego_1 = table.Column<int>(type: "int", nullable: false),
                    id_znajomego_2 = table.Column<int>(type: "int", nullable: false),
                    DataNawiazaniaZnajomosci = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_znajomi", x => new { x.id_znajomego_1, x.id_znajomego_2 });
                    table.ForeignKey(
                        name: "Znajomi_Uzytkownik1",
                        column: x => x.id_znajomego_1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Znajomi_Uzytkownik2",
                        column: x => x.id_znajomego_2,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TypPowiadomienia",
                columns: new[] { "Id", "Nazwa" },
                values: new object[] { 5, "Usunieto cie ze znajomych" });

            migrationBuilder.CreateIndex(
                name: "IX_Znajomi_id_znajomego_2",
                table: "Znajomi",
                column: "id_znajomego_2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Znajomi");

            migrationBuilder.DeleteData(
                table: "TypPowiadomienia",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_wiadomosci_i_typu_wiadomosci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypWiadomosci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypWiadomosci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wiadomosc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_nadawcy = table.Column<int>(type: "int", nullable: false),
                    id_odbiorcy = table.Column<int>(type: "int", nullable: false),
                    DataWyslania = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tresc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IdTypuWiadomosci = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wiadomosc", x => x.Id);
                    table.ForeignKey(
                        name: "Wiadomosc_Nadawca",
                        column: x => x.id_nadawcy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Wiadomosc_Odbiorca",
                        column: x => x.id_odbiorcy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Wiadomosc_Typ_Wiadomosci",
                        column: x => x.IdTypuWiadomosci,
                        principalTable: "TypWiadomosci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TypWiadomosci",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Prywatna" },
                    { 2, "Do gildii" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wiadomosc_id_nadawcy",
                table: "Wiadomosc",
                column: "id_nadawcy");

            migrationBuilder.CreateIndex(
                name: "IX_Wiadomosc_id_odbiorcy",
                table: "Wiadomosc",
                column: "id_odbiorcy");

            migrationBuilder.CreateIndex(
                name: "IX_Wiadomosc_IdTypuWiadomosci",
                table: "Wiadomosc",
                column: "IdTypuWiadomosci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wiadomosc");

            migrationBuilder.DropTable(
                name: "TypWiadomosci");
        }
    }
}

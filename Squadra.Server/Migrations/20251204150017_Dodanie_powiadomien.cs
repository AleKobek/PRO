using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_powiadomien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypPowiadomienia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypPowiadomienia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Powiadomienie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypPowiadomieniaId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    PowiazanyObiektId = table.Column<int>(type: "int", nullable: true),
                    Tresc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataWyslania = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Powiadomienie", x => x.Id);
                    table.ForeignKey(
                        name: "Powiadomienie_TypPowiadomienia",
                        column: x => x.TypPowiadomieniaId,
                        principalTable: "TypPowiadomienia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Powiadomienie_Uzytkownik",
                        column: x => x.UzytkownikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TypPowiadomienia",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Systemowe" },
                    { 2, "Zaproszenie do znajomych" },
                    { 3, "Zaakceptowano zaproszenie do znajomych" },
                    { 4, "Odrzucono zaproszenie do znajomych" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Powiadomienie_TypPowiadomieniaId",
                table: "Powiadomienie",
                column: "TypPowiadomieniaId");

            migrationBuilder.CreateIndex(
                name: "IX_Powiadomienie_UzytkownikId",
                table: "Powiadomienie",
                column: "UzytkownikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Powiadomienie");

            migrationBuilder.DropTable(
                name: "TypPowiadomienia");
        }
    }
}

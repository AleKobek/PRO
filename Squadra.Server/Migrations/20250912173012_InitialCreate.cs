using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jezyk",
                columns: table => new
                {
                    JezykId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jezyk", x => x.JezykId);
                });

            migrationBuilder.CreateTable(
                name: "Kraj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StopienBieglosciJezyka",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Wartosc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopienBieglosciJezyka", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    id_kraju = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                    table.ForeignKey(
                        name: "Region_Kraj",
                        column: x => x.id_kraju,
                        principalTable: "Kraj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profil",
                columns: table => new
                {
                    IdUzytkownika = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Zaimki = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Pseudonim = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    id_regionu = table.Column<int>(type: "int", nullable: true),
                    Awatar = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profil", x => x.IdUzytkownika);
                    table.ForeignKey(
                        name: "Profil_Region",
                        column: x => x.id_regionu,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JezykProfilu",
                columns: table => new
                {
                    id_uzytkownika = table.Column<int>(type: "int", nullable: false),
                    id_jezyka = table.Column<int>(type: "int", nullable: false),
                    id_stopnia_bieglosci = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_jezyk_uzytkownika", x => new { x.id_jezyka, x.id_uzytkownika });
                    table.ForeignKey(
                        name: "JezykProfilu_Jezyk",
                        column: x => x.id_jezyka,
                        principalTable: "Jezyk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "JezykProfilu_Profil",
                        column: x => x.id_uzytkownika,
                        principalTable: "Profil",
                        principalColumn: "IdUzytkownika",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "JezykUzytkownika_StopienBieglosci",
                        column: x => x.id_stopnia_bieglosci,
                        principalTable: "StopienBieglosciJezyka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Uzytkownik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Haslo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumerTelefonu = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DataUrodzenia = table.Column<DateOnly>(type: "date", nullable: true),
                    id_statusu = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownik", x => x.Id);
                    table.ForeignKey(
                        name: "Uzytkownik_Profil",
                        column: x => x.Id,
                        principalTable: "Profil",
                        principalColumn: "IdUzytkownika",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Uzytkownik_Status",
                        column: x => x.id_statusu,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Online" },
                    { 2, "Away" },
                    { 3, "Do not disturb" },
                    { 4, "Invisible" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JezykProfilu_id_stopnia_bieglosci",
                table: "JezykProfilu",
                column: "id_stopnia_bieglosci");

            migrationBuilder.CreateIndex(
                name: "IX_JezykProfilu_id_uzytkownika",
                table: "JezykProfilu",
                column: "id_uzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_Profil_id_regionu",
                table: "Profil",
                column: "id_regionu");

            migrationBuilder.CreateIndex(
                name: "IX_Region_id_kraju",
                table: "Region",
                column: "id_kraju");

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownik_id_statusu",
                table: "Uzytkownik",
                column: "id_statusu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JezykProfilu");

            migrationBuilder.DropTable(
                name: "Uzytkownik");

            migrationBuilder.DropTable(
                name: "Jezyk");

            migrationBuilder.DropTable(
                name: "StopienBieglosciJezyka");

            migrationBuilder.DropTable(
                name: "Profil");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Kraj");
        }
    }
}

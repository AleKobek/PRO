using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Jezyk_id_nie_dziala_moze_teraz_zadziala : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "JezykId", table: "Jezyk", newName: "Id");

            
            migrationBuilder.InsertData(
                table: "Jezyk",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "polski" },
                    { 2, "angielski" },
                    { 3, "niemiecki" }
                });

            migrationBuilder.InsertData(
                table: "Kraj",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Polska" },
                    { 2, "Anglia" }
                });

            migrationBuilder.InsertData(
                table: "StopienBieglosciJezyka",
                columns: new[] { "Id", "Nazwa", "Wartosc" },
                values: new object[,]
                {
                    { 1, "Podstawowy", 0 },
                    { 2, "Srednio-Zaawansowany", 0 },
                    { 3, "Zaawansowany", 0 }
                });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "id_kraju", "Nazwa" },
                values: new object[,]
                {
                    { 1, 1, "Mazowieckie" },
                    { 2, 1, "Wielkopolskie" },
                    { 3, 1, "Dolnoslaskie" },
                    { 4, 1, "Lubelskie" },
                    { 5, 1, "Lubuskie" },
                    { 6, 1, "Podkarpackie" },
                    { 7, 1, "Podlaskie" },
                    { 8, 1, "Zachodniopomorskie" },
                    { 9, 2, "East od England" },
                    { 10, 2, "East Midlands" },
                    { 11, 2, "West Midlands" },
                    { 12, 2, "South West England" },
                    { 13, 2, "South East England" },
                    { 14, 2, "North West England" },
                    { 15, 2, "North East England" },
                    { 16, 2, "Greater London" }
                });

            migrationBuilder.InsertData(
                table: "Profil",
                columns: new[] { "IdUzytkownika", "Awatar", "Opis", "Pseudonim", "id_regionu", "Zaimki" },
                values: new object[] { 1, new byte[0], "Lubię placki!", "Leczo", 1, "she/her" });

            migrationBuilder.InsertData(
                table: "JezykProfilu",
                columns: new[] { "id_jezyka", "id_uzytkownika", "id_stopnia_bieglosci" },
                values: new object[,]
                {
                    { 1, 1, 3 },
                    { 2, 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Uzytkownik",
                columns: new[] { "Id", "DataUrodzenia", "Email", "Haslo", "Login", "NumerTelefonu", "id_statusu" },
                values: new object[] { 1, new DateOnly(2002, 10, 5), "eee@eeee.ee", "123456", "Leczo", null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "JezykProfilu",
                keyColumns: new[] { "id_jezyka", "id_uzytkownika" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "JezykProfilu",
                keyColumns: new[] { "id_jezyka", "id_uzytkownika" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Uzytkownik",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Kraj",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Profil",
                keyColumn: "IdUzytkownika",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Kraj",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}

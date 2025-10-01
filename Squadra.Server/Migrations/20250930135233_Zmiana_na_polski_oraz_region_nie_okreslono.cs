using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_na_polski_oraz_region_nie_okreslono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "polski");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "angielski");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "niemiecki");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nazwa",
                value: "francuski");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nazwa",
                value: "hiszpański");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nazwa",
                value: "japoński");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 7,
                column: "Nazwa",
                value: "rosyjski");

            migrationBuilder.UpdateData(
                table: "Kraj",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Polska");

            migrationBuilder.UpdateData(
                table: "Kraj",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Anglia");

            migrationBuilder.UpdateData(
                table: "Profil",
                keyColumn: "IdUzytkownika",
                keyValue: 1,
                column: "id_regionu",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Nie określono");

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Mazowieckie");

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "id_kraju", "Nazwa" },
                values: new object[] { 1, "Wielkopolskie" });

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 10,
                column: "Nazwa",
                value: "Nie określono");

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "id_kraju", "Nazwa" },
                values: new object[,]
                {
                    { 17, 2, "East od England" },
                    { 18, 2, "East Midlands" }
                });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Dostępny");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Zaraz wracam");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "Nie przeszkadzać");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nazwa",
                value: "Niewidoczny");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Podstawowy");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Średnio-zaawansowany");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "Zaawansowany");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "polish");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "english");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "german");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nazwa",
                value: "french");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nazwa",
                value: "spanish");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nazwa",
                value: "japanese");

            migrationBuilder.UpdateData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 7,
                column: "Nazwa",
                value: "russian");

            migrationBuilder.UpdateData(
                table: "Kraj",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Poland");

            migrationBuilder.UpdateData(
                table: "Kraj",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "England");

            migrationBuilder.UpdateData(
                table: "Profil",
                keyColumn: "IdUzytkownika",
                keyValue: 1,
                column: "id_regionu",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Mazowieckie");

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Wielkopolskie");

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "id_kraju", "Nazwa" },
                values: new object[] { 2, "East od England" });

            migrationBuilder.UpdateData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 10,
                column: "Nazwa",
                value: "East Midlands");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Online");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Away");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "Do not disturb");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nazwa",
                value: "Invisible");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nazwa",
                value: "Basic");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nazwa",
                value: "Intermediate");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "Advanced");
        }
    }
}

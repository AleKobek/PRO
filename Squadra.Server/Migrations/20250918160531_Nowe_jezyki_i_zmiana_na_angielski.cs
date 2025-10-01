using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Nowe_jezyki_i_zmiana_na_angielski : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Jezyk",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 4, "french" },
                    { 5, "spanish" },
                    { 6, "japanese" },
                    { 7, "russian" }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Jezyk",
                keyColumn: "Id",
                keyValue: 7);

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
                value: "Srednio-Zaawansowany");

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nazwa",
                value: "Zaawansowany");
        }
    }
}

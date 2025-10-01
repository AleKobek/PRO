using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class DodatkowyJezykProfiluDlaTestow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "JezykProfilu",
                columns: new[] { "id_jezyka", "id_uzytkownika", "id_stopnia_bieglosci" },
                values: new object[] { 3, 1, 1 });

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 1,
                column: "Wartosc",
                value: 1);

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 2,
                column: "Wartosc",
                value: 2);

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 3,
                column: "Wartosc",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JezykProfilu",
                keyColumns: new[] { "id_jezyka", "id_uzytkownika" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 1,
                column: "Wartosc",
                value: 0);

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 2,
                column: "Wartosc",
                value: 0);

            migrationBuilder.UpdateData(
                table: "StopienBieglosciJezyka",
                keyColumn: "Id",
                keyValue: 3,
                column: "Wartosc",
                value: 0);
        }
    }
}

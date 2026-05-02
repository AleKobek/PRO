using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_wartosci_i_wartosci_liczbowej_statystyki_miejsca_w_druzynie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "wartosc_liczbowa_statystyki",
                table: "Miejsce_w_druzynie",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wartosc_statystyki",
                table: "Miejsce_w_druzynie",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wartosc_liczbowa_statystyki",
                table: "Miejsce_w_druzynie");

            migrationBuilder.DropColumn(
                name: "wartosc_statystyki",
                table: "Miejsce_w_druzynie");
        }
    }
}

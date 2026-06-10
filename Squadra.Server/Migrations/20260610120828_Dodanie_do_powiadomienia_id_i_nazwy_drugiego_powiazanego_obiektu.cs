using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_do_powiadomienia_id_i_nazwy_drugiego_powiazanego_obiektu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_drugiego_powiazanego_obiektu",
                table: "Powiadomienie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nazwa_drugiego_powiazanego_obiektu",
                table: "Powiadomienie",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_drugiego_powiazanego_obiektu",
                table: "Powiadomienie");

            migrationBuilder.DropColumn(
                name: "nazwa_drugiego_powiazanego_obiektu",
                table: "Powiadomienie");
        }
    }
}

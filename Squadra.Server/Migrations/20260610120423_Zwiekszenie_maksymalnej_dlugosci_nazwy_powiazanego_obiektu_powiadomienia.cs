using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zwiekszenie_maksymalnej_dlugosci_nazwy_powiazanego_obiektu_powiadomienia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "nazwa_powiazanego_obiektu",
                table: "Powiadomienie",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "nazwa_powiazanego_obiektu",
                table: "Powiadomienie",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);
        }
    }
}

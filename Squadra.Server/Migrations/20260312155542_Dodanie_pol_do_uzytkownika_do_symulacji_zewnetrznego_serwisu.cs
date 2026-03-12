using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_pol_do_uzytkownika_do_symulacji_zewnetrznego_serwisu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HasloNaZewnetrznymSerwisieHash",
                table: "AspNetUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdNaZewnetrznymSerwisie",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginNaZewnetrznymSerwisie",
                table: "AspNetUsers",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasloNaZewnetrznymSerwisieHash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdNaZewnetrznymSerwisie",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LoginNaZewnetrznymSerwisie",
                table: "AspNetUsers");
        }
    }
}

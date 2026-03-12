using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_id_oraz_loginu_zewnetrznego_serwisu_do_tabeli_uzytkownika : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IdNaZewnetrznymSerwisie",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LoginNaZewnetrznymSerwisie",
                table: "AspNetUsers");
        }
    }
}

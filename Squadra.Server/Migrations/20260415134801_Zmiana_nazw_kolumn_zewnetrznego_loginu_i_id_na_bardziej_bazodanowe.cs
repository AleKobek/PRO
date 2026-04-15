using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_nazw_kolumn_zewnetrznego_loginu_i_id_na_bardziej_bazodanowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoginNaZewnetrznymSerwisie",
                table: "AspNetUsers",
                newName: "login_na_zewnetrznym_serwisie");

            migrationBuilder.RenameColumn(
                name: "IdNaZewnetrznymSerwisie",
                table: "AspNetUsers",
                newName: "id_na_zewnetrznym_serwisie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "login_na_zewnetrznym_serwisie",
                table: "AspNetUsers",
                newName: "LoginNaZewnetrznymSerwisie");

            migrationBuilder.RenameColumn(
                name: "id_na_zewnetrznym_serwisie",
                table: "AspNetUsers",
                newName: "IdNaZewnetrznymSerwisie");
        }
    }
}

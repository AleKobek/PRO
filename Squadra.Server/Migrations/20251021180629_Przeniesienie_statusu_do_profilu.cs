using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Przeniesienie_statusu_do_profilu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Uzytkownik_Status",
                table: "Uzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_Uzytkownik_id_statusu",
                table: "Uzytkownik");

            migrationBuilder.DropColumn(
                name: "id_statusu",
                table: "Uzytkownik");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Uzytkownik",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Haslo",
                table: "Uzytkownik",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<int>(
                name: "id_statusu",
                table: "Profil",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Profil",
                keyColumn: "IdUzytkownika",
                keyValue: 1,
                column: "id_statusu",
                value: 1);

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Nazwa" },
                values: new object[] { 5, "Offline" });

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownik_Email",
                table: "Uzytkownik",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownik_Login",
                table: "Uzytkownik",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profil_id_statusu",
                table: "Profil",
                column: "id_statusu");

            migrationBuilder.AddForeignKey(
                name: "Profil_Status",
                table: "Profil",
                column: "id_statusu",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Profil_Status",
                table: "Profil");

            migrationBuilder.DropIndex(
                name: "IX_Uzytkownik_Email",
                table: "Uzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_Uzytkownik_Login",
                table: "Uzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_Profil_id_statusu",
                table: "Profil");

            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "id_statusu",
                table: "Profil");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Uzytkownik",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Haslo",
                table: "Uzytkownik",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<int>(
                name: "id_statusu",
                table: "Uzytkownik",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "Uzytkownik",
                keyColumn: "Id",
                keyValue: 1,
                column: "id_statusu",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownik_id_statusu",
                table: "Uzytkownik",
                column: "id_statusu");

            migrationBuilder.AddForeignKey(
                name: "Uzytkownik_Status",
                table: "Uzytkownik",
                column: "id_statusu",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_typu_kolumny_wartosc_tabeli_StatystykaUzytkownika_na_string : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Wartosc",
                table: "StatystykaUzytkownika",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // sprawdzamy też, czy konwersja do int jest możliwa, aby uniknąć utraty danych
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM [StatystykaUzytkownika]
                    WHERE TRY_CONVERT(int, [Wartosc]) IS NULL
                )
                BEGIN
                    THROW 50000, 'Rollback aborted: column Wartosc contains non-integer values.', 1;
                END
            ");
        }
    }
}

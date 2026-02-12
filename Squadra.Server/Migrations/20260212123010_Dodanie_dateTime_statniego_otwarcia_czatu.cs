using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_dateTime_statniego_otwarcia_czatu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OstatnieOtwarcieCzatuUzytkownika1",
                table: "Znajomi",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OstatnieOtwarcieCzatuUzytkownika2",
                table: "Znajomi",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OstatnieOtwarcieCzatuUzytkownika1",
                table: "Znajomi");

            migrationBuilder.DropColumn(
                name: "OstatnieOtwarcieCzatuUzytkownika2",
                table: "Znajomi");
        }
    }
}

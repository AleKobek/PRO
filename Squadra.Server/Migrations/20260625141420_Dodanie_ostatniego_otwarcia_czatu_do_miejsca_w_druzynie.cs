using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_ostatniego_otwarcia_czatu_do_miejsca_w_druzynie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ostatnie_otwarcie_czatu",
                table: "Miejsce_w_druzynie",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ostatnie_otwarcie_czatu",
                table: "Miejsce_w_druzynie");
        }
    }
}

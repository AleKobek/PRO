using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_ostatniej_aktywnosci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OstatniaAktywnosc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataUrodzenia", "OstatniaAktywnosc", "PasswordHash", "PhoneNumber" },
                values: new object[] { new DateOnly(2002, 10, 5), null, "AQAAAAIAAYagAAAAEP/9zHapujLFF7cdZeFgdRHL2ueNgRrIRoFWCoyV9eycnY+o/q5y7krDeXOf/7FWkA==", "123456789" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OstatniaAktywnosc",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataUrodzenia", "PasswordHash", "PhoneNumber" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEBLb0gdqt+Nfir8uQHvp0c5ZiQ46E8STEWS8/21xKW9uA+U9Y4IWg3qgo7Tw0zB6pQ==", null });
        }
    }
}

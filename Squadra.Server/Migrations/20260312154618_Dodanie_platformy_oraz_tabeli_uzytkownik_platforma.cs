using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_platformy_oraz_tabeli_uzytkownik_platforma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Platforma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforma", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UzytkownikPlatforma",
                columns: table => new
                {
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    PlatformaId = table.Column<int>(type: "int", nullable: false),
                    PseudonimNaPlatformie = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_uzytkownik_platforma", x => new { x.PlatformaId, x.UzytkownikId });
                    table.ForeignKey(
                        name: "UzytkownikPlatforma_Platforma",
                        column: x => x.PlatformaId,
                        principalTable: "Platforma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "UzytkownikPlatforma_Uzytkownik",
                        column: x => x.UzytkownikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UzytkownikPlatforma_UzytkownikId",
                table: "UzytkownikPlatforma",
                column: "UzytkownikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UzytkownikPlatforma");

            migrationBuilder.DropTable(
                name: "Platforma");
        }
    }
}

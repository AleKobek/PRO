using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_tabeli_rola : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rola",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdGry = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rola", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rola_WspieranaGra_IdGry",
                        column: x => x.IdGry,
                        principalTable: "WspieranaGra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rola_IdGry",
                table: "Rola",
                column: "IdGry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rola");
        }
    }
}

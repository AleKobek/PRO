using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_miejsca_w_druzynie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Miejsce_w_druzynie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_druzyny = table.Column<int>(type: "int", nullable: false),
                    id_uzytkownika = table.Column<int>(type: "int", nullable: true),
                    id_roli = table.Column<int>(type: "int", nullable: true),
                    id_statystyki = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Miejsce_w_druzynie", x => x.Id);
                    table.ForeignKey(
                        name: "Miejsce_W_Druzynie_Druzyna",
                        column: x => x.id_druzyny,
                        principalTable: "Druzyna",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Miejsce_W_Druzynie_Rola",
                        column: x => x.id_roli,
                        principalTable: "Rola",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Miejsce_W_Druzynie_Statystyka",
                        column: x => x.id_statystyki,
                        principalTable: "Statystyka",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Miejsce_W_Druzynie_Uzytkownik",
                        column: x => x.id_uzytkownika,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Miejsce_w_druzynie_id_druzyny",
                table: "Miejsce_w_druzynie",
                column: "id_druzyny");

            migrationBuilder.CreateIndex(
                name: "IX_Miejsce_w_druzynie_id_roli",
                table: "Miejsce_w_druzynie",
                column: "id_roli");

            migrationBuilder.CreateIndex(
                name: "IX_Miejsce_w_druzynie_id_statystyki",
                table: "Miejsce_w_druzynie",
                column: "id_statystyki");

            migrationBuilder.CreateIndex(
                name: "IX_Miejsce_w_druzynie_id_uzytkownika",
                table: "Miejsce_w_druzynie",
                column: "id_uzytkownika");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Miejsce_w_druzynie");
        }
    }
}

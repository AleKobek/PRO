using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Przywrocenie_id_kraju : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Region_Kraj_KrajId",
                table: "Region");

            migrationBuilder.AddForeignKey(
                name: "Region_Kraj",
                table: "Region",
                column: "KrajId",
                principalTable: "Kraj",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Region_Kraj",
                table: "Region");

            migrationBuilder.AddForeignKey(
                name: "FK_Region_Kraj_KrajId",
                table: "Region",
                column: "KrajId",
                principalTable: "Kraj",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

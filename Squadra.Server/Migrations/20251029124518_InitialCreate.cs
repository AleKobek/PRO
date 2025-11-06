using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataUrodzenia = table.Column<DateOnly>(type: "date", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jezyk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jezyk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kraj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StopienBieglosciJezyka",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Wartosc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopienBieglosciJezyka", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    id_kraju = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                    table.ForeignKey(
                        name: "Region_Kraj",
                        column: x => x.id_kraju,
                        principalTable: "Kraj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profil",
                columns: table => new
                {
                    IdUzytkownika = table.Column<int>(type: "int", nullable: false),
                    Zaimki = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Pseudonim = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    id_regionu = table.Column<int>(type: "int", nullable: true),
                    Awatar = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    id_statusu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profil", x => x.IdUzytkownika);
                    table.ForeignKey(
                        name: "FK_Profil_AspNetUsers_IdUzytkownika",
                        column: x => x.IdUzytkownika,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Profil_Region",
                        column: x => x.id_regionu,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Profil_Status",
                        column: x => x.id_statusu,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JezykProfilu",
                columns: table => new
                {
                    id_uzytkownika = table.Column<int>(type: "int", nullable: false),
                    id_jezyka = table.Column<int>(type: "int", nullable: false),
                    id_stopnia_bieglosci = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id_jezyk_uzytkownika", x => new { x.id_jezyka, x.id_uzytkownika });
                    table.ForeignKey(
                        name: "JezykProfilu_Jezyk",
                        column: x => x.id_jezyka,
                        principalTable: "Jezyk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "JezykProfilu_Profil",
                        column: x => x.id_uzytkownika,
                        principalTable: "Profil",
                        principalColumn: "IdUzytkownika",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "JezykUzytkownika_StopienBieglosci",
                        column: x => x.id_stopnia_bieglosci,
                        principalTable: "StopienBieglosciJezyka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DataUrodzenia", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, "00000000-0000-0000-0000-000000000002", null, "eee@eeee.ee", true, false, null, "EEE@EEEE.EE", "LECZO", "AQAAAAIAAYagAAAAEBLb0gdqt+Nfir8uQHvp0c5ZiQ46E8STEWS8/21xKW9uA+U9Y4IWg3qgo7Tw0zB6pQ==", null, false, "00000000-0000-0000-0000-000000000001", false, "Leczo" });

            migrationBuilder.InsertData(
                table: "Jezyk",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "polski" },
                    { 2, "angielski" },
                    { 3, "niemiecki" },
                    { 4, "francuski" },
                    { 5, "hiszpański" },
                    { 6, "japoński" },
                    { 7, "rosyjski" }
                });

            migrationBuilder.InsertData(
                table: "Kraj",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Polska" },
                    { 2, "Anglia" }
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Dostępny" },
                    { 2, "Zaraz wracam" },
                    { 3, "Nie przeszkadzać" },
                    { 4, "Niewidoczny" },
                    { 5, "Offline" }
                });

            migrationBuilder.InsertData(
                table: "StopienBieglosciJezyka",
                columns: new[] { "Id", "Nazwa", "Wartosc" },
                values: new object[,]
                {
                    { 1, "Podstawowy", 1 },
                    { 2, "Średnio-zaawansowany", 2 },
                    { 3, "Zaawansowany", 3 }
                });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "id_kraju", "Nazwa" },
                values: new object[,]
                {
                    { 1, 1, "Nie określono" },
                    { 2, 1, "Mazowieckie" },
                    { 3, 1, "Dolnoslaskie" },
                    { 4, 1, "Lubelskie" },
                    { 5, 1, "Lubuskie" },
                    { 6, 1, "Podkarpackie" },
                    { 7, 1, "Podlaskie" },
                    { 8, 1, "Zachodniopomorskie" },
                    { 9, 1, "Wielkopolskie" },
                    { 10, 2, "Nie określono" },
                    { 11, 2, "West Midlands" },
                    { 12, 2, "South West England" },
                    { 13, 2, "South East England" },
                    { 14, 2, "North West England" },
                    { 15, 2, "North East England" },
                    { 16, 2, "Greater London" },
                    { 17, 2, "East od England" },
                    { 18, 2, "East Midlands" }
                });

            migrationBuilder.InsertData(
                table: "Profil",
                columns: new[] { "IdUzytkownika", "Awatar", "Opis", "Pseudonim", "id_regionu", "id_statusu", "Zaimki" },
                values: new object[] { 1, new byte[0], "Lubię placki!", "Leczo", 2, 1, "she/her" });

            migrationBuilder.InsertData(
                table: "JezykProfilu",
                columns: new[] { "id_jezyka", "id_uzytkownika", "id_stopnia_bieglosci" },
                values: new object[,]
                {
                    { 1, 1, 3 },
                    { 2, 1, 2 },
                    { 3, 1, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JezykProfilu_id_stopnia_bieglosci",
                table: "JezykProfilu",
                column: "id_stopnia_bieglosci");

            migrationBuilder.CreateIndex(
                name: "IX_JezykProfilu_id_uzytkownika",
                table: "JezykProfilu",
                column: "id_uzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_Profil_id_regionu",
                table: "Profil",
                column: "id_regionu");

            migrationBuilder.CreateIndex(
                name: "IX_Profil_id_statusu",
                table: "Profil",
                column: "id_statusu");

            migrationBuilder.CreateIndex(
                name: "IX_Region_id_kraju",
                table: "Region",
                column: "id_kraju");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JezykProfilu");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Jezyk");

            migrationBuilder.DropTable(
                name: "Profil");

            migrationBuilder.DropTable(
                name: "StopienBieglosciJezyka");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Kraj");
        }
    }
}

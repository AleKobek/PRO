﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Zmiana_typu_danych_loga_platformy_na_varbinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE [Platforma]
                  ADD [logo_tmp] varbinary(max) NOT NULL DEFAULT 0x;");
            
            migrationBuilder.Sql(
                @"UPDATE [Platforma]
                  SET [logo_tmp] = CONVERT(varbinary(max), [logo]);");
            
            migrationBuilder.Sql(
                @"ALTER TABLE [Platforma]
                  DROP COLUMN [logo];");
            
            migrationBuilder.Sql(
                @"EXEC sp_rename 'Platforma.logo_tmp', 'logo', 'COLUMN';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE [Platforma]
                  ADD [logo_tmp] nvarchar(max) NOT NULL DEFAULT '';");
            
            migrationBuilder.Sql(
                @"UPDATE [Platforma]
                  SET [logo_tmp] = CONVERT(nvarchar(max), [logo]);");
            
            migrationBuilder.Sql(
                @"ALTER TABLE [Platforma]
                  DROP COLUMN [logo];");
            
            migrationBuilder.Sql(
                @"EXEC sp_rename 'Platforma.logo_tmp', 'logo', 'COLUMN';");
        }
    }
}

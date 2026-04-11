using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squadra.Server.Migrations
{
    /// <inheritdoc />
    public partial class Naprawa_identity_pk_platformy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                 SET XACT_ABORT ON; -- żeby przy błędzie SQL Server automatycznie rollbackował całość
                 BEGIN TRAN;

                 ALTER TABLE [GraNaPlatformie] DROP CONSTRAINT [GraNaPlatformie_Platforma];
                 ALTER TABLE [UzytkownikPlatforma] DROP CONSTRAINT [UzytkownikPlatforma_Platforma];
                 ALTER TABLE [GraUzytkownikaNaPlatformie] DROP CONSTRAINT [GraUzytkownikaNaPlatformie_Platforma];

                 CREATE TABLE [Platforma_tmp](
                     [id] int NOT NULL,
                     [nazwa] nvarchar(40) NOT NULL,
                     [logo] nvarchar(max) NOT NULL,
                     CONSTRAINT [PK_Platforma_tmp] PRIMARY KEY ([id])
                 );

                 INSERT INTO [Platforma_tmp]([id],[nazwa],[logo])
                 SELECT [id],[nazwa],[logo] FROM [Platforma];

                 DROP TABLE [Platforma];
                 EXEC sp_rename 'Platforma_tmp', 'Platforma';
                 
                 ALTER TABLE [GraNaPlatformie]
                     ADD CONSTRAINT [GraNaPlatformie_Platforma]
                     FOREIGN KEY ([id_platformy]) REFERENCES [Platforma]([id]) ON DELETE NO ACTION;

                 ALTER TABLE [UzytkownikPlatforma]
                     ADD CONSTRAINT [UzytkownikPlatforma_Platforma]
                     FOREIGN KEY ([id_platformy]) REFERENCES [Platforma]([id]) ON DELETE NO ACTION;

                 ALTER TABLE [GraUzytkownikaNaPlatformie]
                     ADD CONSTRAINT [GraUzytkownikaNaPlatformie_Platforma]
                     FOREIGN KEY ([id_platformy]) REFERENCES [Platforma]([id]) ON DELETE NO ACTION;

                 COMMIT;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                     SET XACT_ABORT ON; -- żeby przy błędzie SQL Server automatycznie rollbackował całość
                     BEGIN TRAN;

                     ALTER TABLE [GraNaPlatformie] DROP CONSTRAINT [GraNaPlatformie_Platforma];
                     ALTER TABLE [UzytkownikPlatforma] DROP CONSTRAINT [UzytkownikPlatforma_Platforma];
                     ALTER TABLE [GraUzytkownikaNaPlatformie] DROP CONSTRAINT [GraUzytkownikaNaPlatformie_Platforma];

                     CREATE TABLE [Platforma_tmp](
                         [id] int NOT NULL IDENTITY(1,1),
                         [nazwa] nvarchar(40) NOT NULL,
                         [logo] nvarchar(max) NOT NULL,
                         CONSTRAINT [PK_Platforma_tmp] PRIMARY KEY ([id])
                     );

                     SET IDENTITY_INSERT [Platforma_tmp] ON;
                     INSERT INTO [Platforma_tmp]([id],[nazwa],[logo])
                     SELECT [id],[nazwa],[logo] FROM [Platforma];
                     SET IDENTITY_INSERT [Platforma_tmp] OFF;

                     DROP TABLE [Platforma];
                     EXEC sp_rename 'Platforma_tmp', 'Platforma';

                     ALTER TABLE [GraNaPlatformie]
                         ADD CONSTRAINT [GraNaPlatformie_Platforma]
                         FOREIGN KEY ([id_platformy]) REFERENCES [Platforma]([id]) ON DELETE NO ACTION;

                     ALTER TABLE [UzytkownikPlatforma]
                         ADD CONSTRAINT [UzytkownikPlatforma_Platforma]
                         FOREIGN KEY ([id_platformy]) REFERENCES [Platforma]([id]) ON DELETE NO ACTION;

                     ALTER TABLE [GraUzytkownikaNaPlatformie]
                         ADD CONSTRAINT [GraUzytkownikaNaPlatformie_Platforma]
                         FOREIGN KEY ([id_platformy]) REFERENCES [Platforma]([id]) ON DELETE NO ACTION;

                     COMMIT;
                """);
        }
    }
}

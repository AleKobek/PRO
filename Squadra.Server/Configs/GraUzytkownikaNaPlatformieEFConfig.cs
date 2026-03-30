using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.BibliotekaGier.Models;

namespace Squadra.Server.Configs;

public class GraUzytkownikaNaPlatformieEFConfig : IEntityTypeConfiguration<GraUzytkownikaNaPlatformie>
{
    public void Configure(EntityTypeBuilder<GraUzytkownikaNaPlatformie> builder)
    {
        
        builder
            .HasKey(x => new {x.UzytkownikId, x.GraId, x.PlatformaId})
            .HasName("id_gra_uzytkownika_na_platformie");
        
        builder
            .HasOne(x => x.Gra)
            .WithMany(x => x.GraUzytkownikaNaPlatformieCollection)
            .HasForeignKey(x => new {x.UzytkownikId, x.GraId})
            .HasConstraintName("GraUzytkownikaNaPlatformie_GraUzytkownika")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Platforma)
            .WithMany(x => x.GraUzytkownikaNaPlatformieCollection)
            .HasForeignKey(x => x.PlatformaId)
            .HasConstraintName("GraUzytkownikaNaPlatformie_Platforma")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("GraUzytkownikaNaPlatformie");
    }
}
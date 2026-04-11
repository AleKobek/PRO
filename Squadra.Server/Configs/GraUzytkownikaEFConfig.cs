using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.BibliotekaGier.Models;

namespace Squadra.Server.Configs;

public class GraUzytkownikaEFConfig : IEntityTypeConfiguration<GraUzytkownika>
{
    public void Configure(EntityTypeBuilder<GraUzytkownika> builder)
    {
        builder
            .HasKey(x => new {x.UzytkownikId, x.GraId})
            .HasName("id_gra_uzytkownika");
        
        builder
            .Property(x => x.UzytkownikId)
            .HasColumnName("id_uzytkownika");
        
        builder.Property(x => x.GraId)
            .HasColumnName("id_gry");
        
        builder
            .HasOne(x => x.Gra)
            .WithMany(x => x.GraUzytkownikaCollection)
            .HasForeignKey(x => x.GraId)
            .HasConstraintName("GraUzytkownika_Gra")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Uzytkownik)
            .WithMany(x => x.GraUzytkownikaCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("GraUzytkownika_Uzytkownik")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("GraUzytkownika");
    }
}
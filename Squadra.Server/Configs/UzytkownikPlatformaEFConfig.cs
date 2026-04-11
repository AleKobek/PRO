using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Configs;

public class UzytkownikPlatformaEFConfig : IEntityTypeConfiguration<UzytkownikPlatforma>
{
    public void Configure(EntityTypeBuilder<UzytkownikPlatforma> builder)
    {
        
        builder
            .HasKey(x => new {x.PlatformaId, x.UzytkownikId})
            .HasName("id_uzytkownik_platforma");

        builder
            .Property(x => x.PlatformaId)
            .HasColumnName("id_platformy");
        
        builder
            .Property(x => x.UzytkownikId)
            .HasColumnName("id_uzytkownika");
        
        builder
            .Property(x => x.PseudonimNaPlatformie)
            .HasColumnName("pseudonim_na_platformie")
            .HasMaxLength(40)
            .IsRequired();

        builder
            .HasOne<Platforma>(x => x.Platforma)
            .WithMany(x => x.UzytkownikPlatformaCollection)
            .HasForeignKey(x => x.PlatformaId)
            .HasConstraintName("UzytkownikPlatforma_Platforma")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne<Uzytkownik>(x => x.Uzytkownik)
            .WithMany(x => x.UzytkownikPlatformaCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("UzytkownikPlatforma_Uzytkownik")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(UzytkownikPlatforma));
    }
}
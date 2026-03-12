using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;
using Squadra.Server.Modules.Platforma.Models;

namespace Squadra.Server.Configs;

public class UzytkownikPlatformaEFConfig : IEntityTypeConfiguration<UzytkownikPlatforma>
{
    public void Configure(EntityTypeBuilder<UzytkownikPlatforma> builder)
    {
        
        builder
            .HasKey(x => new {x.PlatformaId, x.UzytkownikId})
            .HasName("id_uzytkownik_platforma");
        
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
        
        builder
            .Property(x => x.PseudonimNaPlatformie)
            .HasMaxLength(40)
            .IsRequired();
        
        builder.ToTable(nameof(UzytkownikPlatforma));
    }
}
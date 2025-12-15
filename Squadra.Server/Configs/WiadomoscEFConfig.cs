using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class WiadomoscEFConfig : IEntityTypeConfiguration<Wiadomosc>
{
    public void Configure(EntityTypeBuilder<Wiadomosc> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Tresc)
            .HasMaxLength(1000)
            .IsRequired();

        builder
            .Property(x => x.DataWyslania)
            .IsRequired();

        builder
            .HasOne(x => x.Nadawca)
            .WithMany(x => x.WiadomosciNadaneCollection)
            .HasForeignKey(x => x.IdNadawcy)
            .HasConstraintName("Wiadomosc_Nadawca")
            .OnDelete(DeleteBehavior.Restrict);
        
        
        builder
            .HasOne(x => x.Odbiorca)
            .WithMany(x => x.WiadomosciOdebraneCollection)
            .HasForeignKey(x => x.IdOdbiorcy)
            .HasConstraintName("Wiadomosc_Odbiorca")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.TypWiadomosci)
            .WithMany(x => x.WiadomosciCollection)
            .HasForeignKey(x => x.IdTypuWiadomosci)
            .HasConstraintName("Wiadomosc_Typ_Wiadomosci")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(Wiadomosc));
    }
}
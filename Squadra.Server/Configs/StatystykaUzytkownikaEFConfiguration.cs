using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Configs;

public class StatystykaUzytkownikaEFConfiguration :IEntityTypeConfiguration<StatystykaUzytkownika>
{
    public void Configure(EntityTypeBuilder<StatystykaUzytkownika> builder)
    {
        builder
            .HasKey(x => new {x.UzytkownikId, x.StatystykaId})
            .HasName("id_statystyka_uzytkownika");
        
        builder
            .Property(x => x.Wartosc)
            .IsRequired();
        
        builder
            .HasOne(x => x.Statystyka)
            .WithMany(x => x.StatystykaUzytkownikaCollection)
            .HasForeignKey(x => x.StatystykaId)
            .HasConstraintName("StatystykaUzytkownika_Statystyka")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Uzytkownik)
            .WithMany(x => x.StatystykaUzytkownikaCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("StatystykaUzytkownika_Uzytkownik")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("StatystykaUzytkownika");
    }
}
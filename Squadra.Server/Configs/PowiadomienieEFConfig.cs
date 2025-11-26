using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class PowiadomienieEFConfig : IEntityTypeConfiguration<Powiadomienie>
{
    public void Configure(EntityTypeBuilder<Powiadomienie> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.DataWyslania)
            .IsRequired();
        
        builder
            .Property(x => x.Tresc)
            .HasMaxLength(200)
            .IsRequired();
        
        
        builder
            .HasOne(x => x.Uzytkownik)
            .WithMany(x => x.PowiadomienieCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("Powiadomienie_Uzytkownik")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.TypPowiadomienia)
            .WithMany(x => x.PowiadomienieCollection)
            .HasForeignKey(x => x.TypPowiadomieniaId)
            .HasConstraintName("Powiadomienie_TypPowiadomienia")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(Powiadomienie));
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Powiadomienia.Models;

namespace Squadra.Server.Configs;

public class PowiadomienieEFConfig : IEntityTypeConfiguration<Powiadomienie>
{
    public void Configure(EntityTypeBuilder<Powiadomienie> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.DataWyslania)
            .HasColumnName("data_wyslania")
            .IsRequired();
        
        builder
            .Property(x => x.PowiazanyObiektNazwa)
            .HasColumnName("nazwa_powiazanego_obiektu")
            .HasMaxLength(30);

        builder
            .Property(x => x.Tresc)
            .HasColumnName("tresc")
            .HasMaxLength(200);
        
        
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
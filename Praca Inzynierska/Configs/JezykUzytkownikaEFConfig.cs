using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Configs;

public class JezykUzytkownikaEFConfig : IEntityTypeConfiguration<JezykUzytkownika>
{
    public void Configure(EntityTypeBuilder<JezykUzytkownika> builder)
    {
        
        builder
            .HasKey(x => new {x.JezykId, x.UzytkownikId})
            .HasName("id_jezyk_uzytkownika");
        
        builder
            .HasOne(x => x.Jezyk)
            .WithMany(x => x.JezykUzytkownikaCollection)
            .HasForeignKey(x => x.JezykId)
            .HasConstraintName("JezykUzytkownika_Jezyk")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Uzytkownik)
            .WithMany(x => x.JezykUzytkownikaCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("JezykUzytkownika_Uzytkownik")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.StopienBieglosciJezyka)
            .WithMany(x => x.JezykUzytkownikaCollection)
            .HasForeignKey(x => x.StopienBieglosciId)
            .HasConstraintName("JezykUzytkownika_StopienBieglosci")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(JezykUzytkownika));
    }
}
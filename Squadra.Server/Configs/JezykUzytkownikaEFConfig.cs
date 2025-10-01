using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class JezykUzytkownikaEFConfig : IEntityTypeConfiguration<JezykProfilu>
{
    public void Configure(EntityTypeBuilder<JezykProfilu> builder)
    {
        
        builder
            .HasKey(x => new {x.JezykId, x.UzytkownikId})
            .HasName("id_jezyk_uzytkownika");
        
        builder
            .HasOne(x => x.Jezyk)
            .WithMany(x => x.JezykProfiluCollection)
            .HasForeignKey(x => x.JezykId)
            .HasConstraintName("JezykProfilu_Jezyk")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Profil)
            .WithMany(x => x.JezykUzytkownikaCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("JezykProfilu_Profil")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.StopienBieglosciJezyka)
            .WithMany(x => x.JezykUzytkownikaCollection)
            .HasForeignKey(x => x.StopienBieglosciId)
            .HasConstraintName("JezykUzytkownika_StopienBieglosci")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(JezykProfilu));
    }
}
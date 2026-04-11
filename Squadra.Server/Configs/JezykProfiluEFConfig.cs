using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Configs;

public class JezykProfiluEFConfig : IEntityTypeConfiguration<JezykProfilu>
{
    public void Configure(EntityTypeBuilder<JezykProfilu> builder)
    {
        
        builder
            .HasKey(x => new {x.JezykId, x.UzytkownikId})
            .HasName("id_jezyk_profilu");

        builder
            .Property(x => x.JezykId)
            .HasColumnName("id_jezyka");
        
        builder
            .Property(x => x.UzytkownikId)
            .HasColumnName("id_uzytkownika");
        
        builder
            .Property(x => x.StopienBieglosciId)
            .HasColumnName("id_stopnia_bieglosci");
        
        builder
            .HasOne(x => x.Jezyk)
            .WithMany(x => x.JezykProfiluCollection)
            .HasForeignKey(x => x.JezykId)
            .HasConstraintName("JezykProfilu_Jezyk")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Profil)
            .WithMany(x => x.JezykProfiluCollection)
            .HasForeignKey(x => x.UzytkownikId)
            .HasConstraintName("JezykProfilu_Profil")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.StopienBieglosciJezyka)
            .WithMany(x => x.JezykProfiluCollection)
            .HasForeignKey(x => x.StopienBieglosciId)
            .HasConstraintName("JezykProfilu_StopienBieglosci")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(JezykProfilu));
    }
}
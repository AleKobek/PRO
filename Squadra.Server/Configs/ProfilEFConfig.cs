using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class ProfilEFConfig : IEntityTypeConfiguration<Profil>
{
    public void Configure(EntityTypeBuilder<Profil> builder)
    {
        builder
            .HasKey(x => x.IdUzytkownika);

        builder
            .Property(x => x.Zaimki)
            .HasMaxLength(10);
        
        builder
            .Property(x => x.Opis)
            .HasMaxLength(100);

        builder
            .Property(x => x.Pseudonim)
            .HasMaxLength(20)
            .IsRequired();
        
        builder
            .HasOne(x => x.Region)
            .WithMany(x => x.ProfilCollection)
            .HasForeignKey(x => x.RegionId)
            .HasConstraintName("Profil_Region")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(Profil));
    }
}
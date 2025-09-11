using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Squadra;

public class ProfilEFConfig : IEntityTypeConfiguration<Profil>
{
    public void Configure(EntityTypeBuilder<Profil> builder)
    {
        builder
            .HasKey(x => x.IdUzytkownika)
            .HasName("id_uzytkownika");

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
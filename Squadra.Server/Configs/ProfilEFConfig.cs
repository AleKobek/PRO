using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Configs;

public class ProfilEFConfig : IEntityTypeConfiguration<Profil>
{
    public void Configure(EntityTypeBuilder<Profil> builder)
    {
        builder
            .HasKey(x => x.IdUzytkownika);
        
        builder
            .Property(x => x.IdUzytkownika)
            .HasColumnName("id_uzytkownika")
            .ValueGeneratedNever();

        builder
            .Property(x => x.Zaimki)
            .HasColumnName("zaimki")
            .HasMaxLength(30);
        
        builder
            .Property(x => x.Opis)
            .HasColumnName("opis")
            .HasMaxLength(300);

        builder
            .Property(x => x.Pseudonim)
            .HasMaxLength(20)
            .HasColumnName("pseudonim")
            .IsRequired();
        
        builder
            .HasOne(x => x.Region)
            .WithMany(x => x.ProfilCollection)
            .HasForeignKey(x => x.RegionId)
            .HasConstraintName("Profil_Region")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(p => p.Uzytkownik)
            .WithOne(u => u.Profil)
            .HasForeignKey<Profil>(p => p.IdUzytkownika)
            .IsRequired(false) // pozwala tworzyć Uzytkownik bez Profilu od razu, dodajemy od razu ręcznie
            .OnDelete(DeleteBehavior.Cascade);

        
        builder
            .HasOne(x => x.Status)
            .WithMany(x => x.ProfilCollection)
            .HasForeignKey(x => x.StatusId)
            .HasConstraintName("Profil_Status")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(Profil));
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Configs;

public class DruzynaEFConfig : IEntityTypeConfiguration<Druzyna>
{
    public void Configure(EntityTypeBuilder<Druzyna> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(40)
            .IsRequired();
        
        builder
            .Property(x => x.GraId)
            .HasColumnName("id_gry")
            .IsRequired();
        
        builder
            .Property(x => x.KapitanId)
            .HasColumnName("id_kapitana")
            .IsRequired();
        
        builder
            .Property(x => x.CzyPubliczna)
            .HasColumnName("czy_publiczna")
            .HasDefaultValue(true)
            .IsRequired();
        
        builder
            .Property(x => x.Opis)
            .HasColumnName("opis")
            .HasMaxLength(300);
        
        builder
            .Property(x => x.NastrojRozgrywkiId)
            .HasColumnName("id_nastroju_rozgrywki")
            .HasDefaultValue(1)
            .IsRequired();
        
        builder
            .Property(x => x.WymaganyJezykId)
            .HasColumnName("id_wymaganego_jezyka");
        
        builder
            .Property(x => x.WymaganyStopienBieglosciJezykaId)
            .HasColumnName("id_wymaganego_stopnia_bieglosci_jezyka");
        
        builder
            .Property(x => x.PlatformaId)
            .HasColumnName("id_platformy");
        
        builder
            .Property(x => x.CzyZintegrowano)
            .HasColumnName("czy_zintegrowano")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasOne(x => x.Gra)
            .WithMany(x => x.DruzynaCollection)
            .HasForeignKey(x => x.GraId)
            .HasConstraintName("Druzyna_Gra")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Kapitan)
            .WithMany(x => x.DowodzoneDruzynyCollection)
            .HasForeignKey(x => x.KapitanId)
            .HasConstraintName("Druzyna_Kapitan")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NastrojRozgrywki)
            .WithMany(x=> x.DruzynaCollection)
            .HasForeignKey(x => x.NastrojRozgrywkiId)
            .HasConstraintName("Druzyna_Nastroj_Rozgrywki")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WymaganyJezyk)
            .WithMany(x => x.DruzynaCollection)
            .HasConstraintName("Druzyna_Wymagany_Jezyk")
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(x => x.WymaganyJezykId);
        
        builder.HasOne(x => x.Platforma)
            .WithMany(x => x.DruzynaCollection)
            .HasConstraintName("Druzyna_Platforma")
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(x => x.PlatformaId);
        
        builder.HasOne(x => x.WymaganyStopienBieglosciJezyka)
            .WithMany(x => x.DruzynaCollection)
            .HasConstraintName("Druzyna_Wymagany_Stopien_Bieglosci_Jezyka")
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(x => x.WymaganyStopienBieglosciJezykaId);
        
        builder.ToTable("Druzyna");
    }
}
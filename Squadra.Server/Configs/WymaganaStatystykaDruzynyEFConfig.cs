using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Configs;

public class WymaganaStatystykaDruzynyEFConfig : IEntityTypeConfiguration<WymaganaStatystykaDruzyny>
{
    public void Configure(EntityTypeBuilder<WymaganaStatystykaDruzyny> builder)
    {
        builder.HasKey(x => new  { x.DruzynaId, x.StatystykaId });
        
        builder
            .HasOne(x => x.Druzyna)
            .WithMany(x => x.WymaganaStatystykaDruzynyCollection)
            .HasForeignKey(x => x.DruzynaId)
            .HasConstraintName("WymaganaStatystykaDruzyny_Druzyna")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Statystyka)
            .WithMany(x => x.WymaganaStatystykaDruzynyCollection)
            .HasForeignKey(x => x.StatystykaId)
            .HasConstraintName("Wymagana_Statystyka_Druzyny_Statystyka")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .Property(x => x.Wartosc)
            .HasMaxLength(20)
            .HasColumnName("wartosc");
        
        builder
            .Property(x => x.PorownywalnaWartoscLiczbowa)
            .HasColumnName("porownywalna_wartosc_liczbowa");
        
        builder.ToTable("Wymagana_Statystyka_Druzyny");
    }
}
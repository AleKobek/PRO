using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Configs;

public class RangaEFConfig : IEntityTypeConfiguration<Ranga>
{
    public void Configure(EntityTypeBuilder<Ranga> builder)
    {
        builder
            .HasKey(r => new { r.StatystykaId, r.WartoscLiczbowa});
        
        builder
            .Property(r => r.StatystykaId)
            .HasColumnName("id_statystyki")
            .ValueGeneratedNever();
        
        builder
            .Property(r => r.WartoscLiczbowa)
            .HasColumnName("wartosc_liczbowa")
            .ValueGeneratedNever();

        builder
            .Property(r => r.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(40)
            .IsRequired();
        
        builder.HasOne(r => r.Statystyka)
            .WithMany(s => s.RangaCollection)
            .HasForeignKey(r => r.StatystykaId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Ranga");
    }
}
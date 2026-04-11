using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Configs;

public class WspieranaGraEFConfig : IEntityTypeConfiguration<WspieranaGra>
{
    public void Configure(EntityTypeBuilder<WspieranaGra> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder
            .Property(x => x.Tytul)
            .HasColumnName("tytul")
            .HasMaxLength(60)
            .IsRequired();
        
        builder
            .Property(x => x.Wydawca)
            .HasColumnName("wydawca")
            .HasMaxLength(30)
            .IsRequired();
        
        builder
            .Property(x => x.Gatunek)
            .HasColumnName("gatunek")
            .HasMaxLength(30)
            .IsRequired();
            
        
        builder.ToTable("WspieranaGra");
    }
}
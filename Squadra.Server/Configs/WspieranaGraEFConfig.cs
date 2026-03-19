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
            .ValueGeneratedNever();

        builder
            .Property(x => x.Tytul)
            .HasMaxLength(60)
            .IsRequired();
        
        builder
            .Property(x => x.Wydawca)
            .HasMaxLength(30)
            .IsRequired();
        
        builder
            .Property(x => x.Gatunek)
            .HasMaxLength(30)
            .IsRequired();
            
        
        builder.ToTable("WspieranaGra");
    }
}
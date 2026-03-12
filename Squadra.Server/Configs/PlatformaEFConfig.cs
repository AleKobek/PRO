using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Platforma.Models;

namespace Squadra.Server.Configs;

public class PlatformaEFConfig : IEntityTypeConfiguration<Platforma>
{
    public void Configure(EntityTypeBuilder<Platforma> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(40)
            .IsRequired();
        
        builder
            .Property(x => x.Logo)
            .IsRequired();
        
        builder.ToTable(nameof(Platforma));
    }
}
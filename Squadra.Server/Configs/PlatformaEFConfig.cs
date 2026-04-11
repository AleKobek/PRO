using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Configs;

public class PlatformaEFConfig : IEntityTypeConfiguration<Platforma>
{
    public void Configure(EntityTypeBuilder<Platforma> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id");
        
        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(40)
            .IsRequired();
        
        builder
            .Property(x => x.Logo)
            .HasColumnName("logo")
            .IsRequired();
        
        builder.ToTable(nameof(Platforma));
    }
}
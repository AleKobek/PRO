using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class TypPowiadomieniaEFConfig : IEntityTypeConfiguration<TypPowiadomienia>
{
    public void Configure(EntityTypeBuilder<TypPowiadomienia> builder)
    {
        
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Nazwa)
            .IsRequired();
        
        builder.ToTable(nameof(TypPowiadomienia));
    }
}
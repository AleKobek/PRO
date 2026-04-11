using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Powiadomienia.Models;

namespace Squadra.Server.Configs;

public class TypPowiadomieniaEFConfig : IEntityTypeConfiguration<TypPowiadomienia>
{
    public void Configure(EntityTypeBuilder<TypPowiadomienia> builder)
    {
        
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.ToTable(nameof(TypPowiadomienia));
    }
}
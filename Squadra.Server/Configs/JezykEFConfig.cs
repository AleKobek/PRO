using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Configs;

public class JezykEFConfig : IEntityTypeConfiguration<Jezyk>
{
    public void Configure(EntityTypeBuilder<Jezyk> builder)
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
            .HasMaxLength(20)
            .IsRequired();
        
        builder.ToTable(nameof(Jezyk));
    }
}
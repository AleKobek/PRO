using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Configs;

public class RegionEFConfig : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
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
        
        builder
            .HasOne(x => x.Kraj)
            .WithMany(x => x.RegionCollection)
            .HasForeignKey(x => x.KrajId)
            .HasConstraintName("Region_Kraj")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(Region));
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Squadra;

public class RegionEFConfig : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {

        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.Nazwa)
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
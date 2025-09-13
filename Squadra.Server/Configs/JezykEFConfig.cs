using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Squadra;

public class JezykEFConfig : IEntityTypeConfiguration<Jezyk>
{
    public void Configure(EntityTypeBuilder<Jezyk> builder)
    {

        builder
            .HasKey(x => x.JezykId);
        
        builder
            .Property(x => x.JezykId)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.ToTable(nameof(Jezyk));
    }
}
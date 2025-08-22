using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Configs;

public class KrajEFConfig :  IEntityTypeConfiguration<Kraj>
{
    public void Configure(EntityTypeBuilder<Kraj> builder)
    {
        
        builder
            .HasKey(x => x.Id)
            .HasName("id");
        
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.ToTable(nameof(Kraj));
    }
}
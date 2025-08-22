using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Configs;

public class StopienBieglosciJezykaEFConfig : IEntityTypeConfiguration<StopienBieglosciJezyka>
{
    public void Configure(EntityTypeBuilder<StopienBieglosciJezyka> builder)
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

        builder
            .Property(x => x.Wartosc)
            .IsRequired();
        
        builder.ToTable(nameof(StopienBieglosciJezyka));
    }
}
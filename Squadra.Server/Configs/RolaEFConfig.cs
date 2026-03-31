using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Configs;

public class RolaEFConfig : IEntityTypeConfiguration<Rola>
{
    public void Configure(EntityTypeBuilder<Rola> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(20)
            .IsRequired();

        builder
            .HasOne(x => x.Gra)
            .WithMany(g => g.RolaCollection)
            .HasForeignKey(x => x.IdGry)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Rola");
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Configs;

public class StatystykaEFConfig : IEntityTypeConfiguration<Statystyka>
{
    public void Configure(EntityTypeBuilder<Statystyka> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .HasOne(x => x.Kategoria)
            .WithMany(x => x.StatystykaCollection)
            .HasForeignKey(x => x.KategoriaId)
            .HasConstraintName("Statystyka_Kategoria")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Rola)
            .WithMany(x => x.StatystykaCollection)
            .HasForeignKey(x => x.RolaId)
            .HasConstraintName("Statystyka_Rola")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Statystyka");
    }
}
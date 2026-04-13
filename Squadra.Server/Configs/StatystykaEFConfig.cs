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
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(30)
            .IsRequired();
        
        builder
            .Property(x => x.KategoriaId)
            .HasColumnName("id_kategorii")
            .IsRequired();

        builder
            .Property(x => x.RolaId)
            .HasColumnName("id_roli");
        
        builder
            .Property(x => x.CzyToCzasRozgrywki)
            .HasColumnName("czy_to_czas_rozgrywki")
            .IsRequired()
            .HasDefaultValue(false);

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
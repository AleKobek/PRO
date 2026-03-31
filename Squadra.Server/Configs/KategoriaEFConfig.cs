using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Configs;

public class KategoriaEFConfig : IEntityTypeConfiguration<Kategoria>
{
    public void Configure(EntityTypeBuilder<Kategoria> builder)
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
            .Property(x => x.IdGry)
            .IsRequired();
        
        builder
                .Property(x => x.CzyToCzasRozgrywki)
                .IsRequired()
                .HasDefaultValue(false);
        
        builder
            .HasOne(x => x.Gra)
            .WithMany(x => x.KategoriaCollection)
            .HasForeignKey(x => x.IdGry)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Kategoria");
    }
}
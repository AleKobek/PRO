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
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(30)
            .IsRequired();
        
        builder
            .Property(x => x.IdGry)
            .HasColumnName("id_gry")
            .IsRequired();
        
        builder
            .HasOne(x => x.Gra)
            .WithMany(x => x.KategoriaCollection)
            .HasForeignKey(x => x.IdGry)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Kategoria");
    }
}
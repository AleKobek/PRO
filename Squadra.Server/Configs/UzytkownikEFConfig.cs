using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class UzytkownikEFConfig : IEntityTypeConfiguration<Uzytkownik>
{
    public void Configure(EntityTypeBuilder<Uzytkownik> builder)
    {
        builder
            .HasKey(x =>x.Id);
        
        builder
            .Property(x => x.Login)
            .HasMaxLength(64)
            .IsRequired();
        
        builder.HasIndex(x => x.Login).IsUnique();
        
        builder
            .Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();
            
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder
            .Property(x => x.Email)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property(x => x.NumerTelefonu)
            .HasMaxLength(15);
        
        
        builder
            .HasOne(x => x.Profil)
            .WithOne(x => x.Uzytkownik)
            .HasForeignKey<Uzytkownik>(x => x.Id)
            .HasConstraintName("Uzytkownik_Profil")
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(nameof(Uzytkownik));
    }
}
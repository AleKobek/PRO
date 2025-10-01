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
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Login)
            .HasMaxLength(20)
            .IsRequired();
        
        builder
            .Property(x => x.Email)
            .HasMaxLength(20)
            .IsRequired();
            
        
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

        builder
            .Property(x => x.StatusId)
            .HasDefaultValue(1);
        
        builder
            .HasOne(x => x.Status)
            .WithMany(x => x.UzytkownikCollection)
            .HasForeignKey(x => x.StatusId)
            .HasConstraintName("Uzytkownik_Status")
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(nameof(Uzytkownik));
    }
}
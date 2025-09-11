using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Squadra;

public class UzytkownikEFConfig : IEntityTypeConfiguration<Uzytkownik>
{
    public void Configure(EntityTypeBuilder<Uzytkownik> builder)
    {
        builder
            .HasKey(x =>x.Id)
            .HasName("id");
        
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
            .HasOne(x => x.Status)
            .WithMany(x => x.UzytkownikCollection)
            .HasForeignKey(x => x.StatusId)
            .HasConstraintName("Uzytkownik_Status")
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(nameof(Uzytkownik));
    }
}
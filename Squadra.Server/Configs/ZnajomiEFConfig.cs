using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class ZnajomiEFConfig : IEntityTypeConfiguration<Znajomi>
{
    public void Configure(EntityTypeBuilder<Znajomi> builder)
    {
        builder.
            HasKey(x => new {x.IdUzytkownika1, x.IdUzytkownika2})
            .HasName("id_znajomi");
        
        builder
            .HasOne(x => x.Uzytkownik1)
            .WithMany(x => x.ZnajomiJakoPierwszyCollection)
            .HasForeignKey(x => x.IdUzytkownika1)
            .HasConstraintName("Znajomi_Uzytkownik1")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Uzytkownik2)
            .WithMany(x => x.ZnajomiJakoDrugiCollection)
            .HasForeignKey(x => x.IdUzytkownika2)
            .HasConstraintName("Znajomi_Uzytkownik2")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(nameof(Znajomi));
    }
}
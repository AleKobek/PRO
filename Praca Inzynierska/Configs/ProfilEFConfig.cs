using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Configs;

public class ProfilEFConfig : IEntityTypeConfiguration<Profil>
{
    public void Configure(EntityTypeBuilder<Profil> builder)
    {
        builder
            .HasKey(x => x.IdUzytkownika)
            .HasName("id_uzytkownika");

        builder
            .Property(x => x.Zaimki)
            .HasMaxLength(10);
        
        builder
            .Property(x => x.Opis)
            .HasMaxLength(100);
        
        
        builder.ToTable(nameof(Profil));
    }
}
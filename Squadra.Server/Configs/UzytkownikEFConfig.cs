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
            .Property(x => x.UserName)
            .HasMaxLength(64)
            .IsRequired();
        
        builder
            .Property(x => x.NormalizedUserName)
            .HasMaxLength(64)
            .IsRequired();
        
        builder.HasIndex(x => x.NormalizedUserName).IsUnique();
        
        builder
            .Property(x => x.Email)
            .HasMaxLength(250)
            .IsRequired();
        
        builder
            .Property(x => x.NormalizedEmail)
            .HasMaxLength(250)
            .IsRequired();
            
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
        

        builder
            .Property(x => x.PhoneNumber)
            .HasMaxLength(15);
    }
}
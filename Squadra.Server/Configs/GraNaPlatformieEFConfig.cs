using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Configs;

public class GraNaPlatformieEFConfig : IEntityTypeConfiguration<GraNaPlatformie>
{
    public void Configure(EntityTypeBuilder<GraNaPlatformie> builder)
    {
        builder
            .HasKey(x => new {x.IdWspieranejGry, x.IdPlatformy})
            .HasName("id_gra_na_platformie");
        
        builder
            .Property(x => x.IdWspieranejGry)
            .HasColumnName("id_gry");
        
        builder
            .Property(x => x.IdPlatformy)
            .HasColumnName("id_platformy");
        
        builder
            .HasOne(x => x.WspieranaGra)
            .WithMany(x => x.GraNaPlatformieCollection)
            .HasForeignKey(x => x.IdWspieranejGry)
            .HasConstraintName("GraNaPlatformie_WspieranaGra")
            .OnDelete(DeleteBehavior.Restrict);
            
        builder
            .HasOne(x => x.Platforma)
            .WithMany(x => x.GraNaPlatformieCollection)
            .HasForeignKey(x => x.IdPlatformy)
            .HasConstraintName("GraNaPlatformie_Platforma")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("GraNaPlatformie");
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Models;

namespace Squadra.Server.Configs;

public class TypWiadomosciEFConfig : IEntityTypeConfiguration<TypWiadomosci>
{
    public void Configure(EntityTypeBuilder<TypWiadomosci> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.ToTable(nameof(TypWiadomosci));
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Wiadomosci.Models;

namespace Squadra.Server.Configs;

public class TypWiadomosciEFConfig : IEntityTypeConfiguration<TypWiadomosci>
{
    public void Configure(EntityTypeBuilder<TypWiadomosci> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(50)
            .IsRequired();
        
        builder.ToTable(nameof(TypWiadomosci));
    }
}
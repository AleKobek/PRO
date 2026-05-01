using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Configs;

public class NastrojRozgrywkiEFConfig : IEntityTypeConfiguration<NastrojRozgrywki>
{
    public void Configure(EntityTypeBuilder<NastrojRozgrywki> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.Nazwa)
            .HasColumnName("nazwa")
            .HasMaxLength(40)
            .IsRequired();

        builder.ToTable("Nastroj_Rozgrywki");
    }
}
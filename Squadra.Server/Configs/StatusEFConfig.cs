using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Squadra;

public class StatusEFConfig : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Nazwa)
            .HasMaxLength(20);
        
        builder.ToTable(nameof(Status));
    }
}
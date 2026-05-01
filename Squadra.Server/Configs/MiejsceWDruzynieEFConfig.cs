using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Configs;

public class MiejsceWDruzynieEFConfig : IEntityTypeConfiguration<MiejsceWDruzynie>
{
    public void Configure(EntityTypeBuilder<MiejsceWDruzynie> builder)
    {
        builder
            .HasKey(k => k.Id);
        
        builder
            .Property(k => k.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(x => x.DruzynaId)
            .HasColumnName("id_druzyny")
            .IsRequired();
        
        builder
            .Property(x => x.UzytkownikId)
            .HasColumnName("id_uzytkownika");
        
        builder
            .Property(x => x.RolaId)
            .HasColumnName("id_roli");
        
        builder
            .Property(x => x.StatystykaId)
            .HasColumnName("id_statystyki");
        
        builder
            .HasOne(m => m.Druzyna)
            .WithMany(d => d.MiejsceWDruzynieCollection)
            .HasForeignKey(m => m.DruzynaId)
            .HasConstraintName("Miejsce_W_Druzynie_Druzyna")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(m => m.Uzytkownik)
            .WithMany(d => d.MiejsceWDruzynieCollection)
            .HasForeignKey(m => m.UzytkownikId)
            .HasConstraintName("Miejsce_W_Druzynie_Uzytkownik")
            .OnDelete(DeleteBehavior.Restrict);
            
        builder
            .HasOne(x => x.Rola)
            .WithMany(d => d.MiejsceWDruzynieCollection)
            .HasForeignKey(x => x.RolaId)
            .HasConstraintName("Miejsce_W_Druzynie_Rola")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Statystyka)
            .WithMany(d => d.MiejsceWDruzynieCollection)
            .HasForeignKey(x => x.StatystykaId)
            .HasConstraintName("Miejsce_W_Druzynie_Statystyka")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Miejsce_w_druzynie");
    }
}
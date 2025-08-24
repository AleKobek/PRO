using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Configs;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Context;

public class AppDbContext : DbContext 
{
    public DbSet<Uzytkownik> Uzytkownik { get; set; } = null!;
    public DbSet<Kraj> Kraj { get; set; } = null!;
    public DbSet<Jezyk> Jezyk { get; set; } = null!;
    public DbSet<JezykUzytkownika> JezykUzytkownika { get; set; } = null!;
    public DbSet<Profil> Profil { get; set; } = null!;
    public DbSet<Region> Region { get; set; } = null!;
    public DbSet<StopienBieglosciJezyka> StopienBieglosciJezyka { get; set; } = null!;
    
    public AppDbContext(){}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UzytkownikEFConfig).Assembly);
    }
}
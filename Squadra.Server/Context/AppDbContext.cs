using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class AppDbContext : DbContext 
{
    public DbSet<Uzytkownik> Uzytkownik { get; set; } = null!;
    public DbSet<Kraj> Kraj { get; set; } = null!;
    public DbSet<Jezyk> Jezyk { get; set; } = null!;
    
    public DbSet<JezykProfilu> JezykProfilu { get; set; } = null!;
    public DbSet<Profil> Profil { get; set; } = null!;
    public DbSet<Region> Region { get; set; } = null!;
    public DbSet<StopienBieglosciJezyka> StopienBieglosciJezyka { get; set; } = null!;
    
    public DbSet<Status> Status { get; set; } = null!;
    
    public AppDbContext(){}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Nazwa = "Online" },
            new Status { Id = 2, Nazwa = "Away" },
            new Status { Id = 3, Nazwa = "Do not disturb" },
            new Status { Id = 4, Nazwa = "Invisible" }
        );
        
        

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UzytkownikEFConfig).Assembly);
    }
}
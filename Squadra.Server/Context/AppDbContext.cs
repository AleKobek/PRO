using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Configs;
using Squadra.Server.Models;

namespace Squadra.Server.Context;

public class AppDbContext : IdentityDbContext<Uzytkownik, IdentityRole<int>, int>
 
{
    public DbSet<Uzytkownik> Uzytkownik { get; set; } = null!;
    public DbSet<Kraj> Kraj { get; set; } = null!;
    public DbSet<Jezyk> Jezyk { get; set; } = null!;
    
    public DbSet<JezykProfilu> JezykProfilu { get; set; } = null!;
    public DbSet<Profil> Profil { get; set; } = null!;
    public DbSet<Region> Region { get; set; } = null!;
    public DbSet<StopienBieglosciJezyka> StopienBieglosciJezyka { get; set; } = null!;
    
    public DbSet<Status> Status { get; set; } = null!;
    
    public DbSet<Powiadomienie> Powiadomienie { get; set; } = null!;
    public DbSet<TypPowiadomienia> TypPowiadomienia { get; set; } = null!;
    
    public DbSet<Znajomi> Znajomi { get; set; } = null!;
    
    public AppDbContext(){}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Nazwa = "Dostępny" },
            new Status { Id = 2, Nazwa = "Zaraz wracam" },
            new Status { Id = 3, Nazwa = "Nie przeszkadzać" },
            new Status { Id = 4, Nazwa = "Niewidoczny" },
            new Status { Id = 5, Nazwa = "Offline" }
        );

        modelBuilder.Entity<StopienBieglosciJezyka>().HasData(
            new StopienBieglosciJezyka { Id = 1, Nazwa = "Podstawowy", Wartosc = 1 },
            new StopienBieglosciJezyka { Id = 2, Nazwa = "Średnio-zaawansowany", Wartosc = 2 },
            new StopienBieglosciJezyka { Id = 3, Nazwa = "Zaawansowany", Wartosc = 3 }
        );

        modelBuilder.Entity<Jezyk>().HasData(
            new Jezyk {Id = 1, Nazwa = "polski"},
            new Jezyk {Id = 2, Nazwa = "angielski"},
            new Jezyk {Id = 3, Nazwa = "niemiecki"},
            new Jezyk {Id = 4, Nazwa = "francuski"},
            new Jezyk {Id = 5, Nazwa = "hiszpański"},
            new Jezyk {Id = 6, Nazwa = "japoński"},
            new Jezyk {Id = 7, Nazwa = "rosyjski"}
        );

        modelBuilder.Entity<Kraj>().HasData(
            new Kraj { Id = 1, Nazwa = "Polska" },
            new Kraj { Id = 2, Nazwa = "Anglia" }
        );


        modelBuilder.Entity<Region>().HasData(
            // Polska
            new Region { Id = 1, Nazwa = "Nie określono", KrajId = 1 },
            new Region { Id = 2, Nazwa = "Mazowieckie", KrajId = 1 },
            new Region {Id = 3, Nazwa = "Dolnoslaskie", KrajId = 1},
            new Region {Id = 4, Nazwa = "Lubelskie", KrajId = 1},
            new Region {Id = 5, Nazwa = "Lubuskie", KrajId = 1},
            new Region {Id = 6, Nazwa = "Podkarpackie", KrajId = 1},
            new Region {Id = 7, Nazwa = "Podlaskie", KrajId = 1},
            new Region {Id = 8, Nazwa = "Zachodniopomorskie", KrajId = 1},
            new Region {Id = 9, Nazwa = "Wielkopolskie", KrajId = 1},
            // Anglia
            new Region {Id = 10, Nazwa = "Nie określono", KrajId = 2},
            new Region {Id = 11, Nazwa = "West Midlands", KrajId = 2},
            new Region {Id = 12, Nazwa = "South West England", KrajId = 2},
            new Region {Id = 13, Nazwa = "South East England", KrajId = 2},
            new Region {Id = 14, Nazwa = "North West England", KrajId = 2},
            new Region {Id = 15, Nazwa = "North East England", KrajId = 2},
            new Region {Id = 16, Nazwa = "Greater London", KrajId = 2},
            new Region {Id = 17, Nazwa = "East od England", KrajId = 2},
            new Region {Id = 18, Nazwa = "East Midlands", KrajId = 2}
        );

        modelBuilder.Entity<TypPowiadomienia>().HasData(
            new TypPowiadomienia {Id = 1, Nazwa = "Systemowe"},
            new TypPowiadomienia {Id = 2, Nazwa = "Zaproszenie do znajomych"},
            new TypPowiadomienia {Id = 3, Nazwa = "Zaakceptowano zaproszenie do znajomych"},
            new TypPowiadomienia {Id = 4, Nazwa = "Odrzucono zaproszenie do znajomych"}
        );
        

        // modelBuilder.Entity<JezykProfilu>().HasData(
        //     new JezykProfilu {UzytkownikId = 1, JezykId = 1, StopienBieglosciId = 3},
        //     new JezykProfilu {UzytkownikId = 1, JezykId = 2, StopienBieglosciId = 2},
        //     new JezykProfilu {UzytkownikId = 1, JezykId = 3, StopienBieglosciId = 1}
        // );
        //
        // modelBuilder.Entity<Uzytkownik>().HasData(
        //     new Uzytkownik
        // {
        //     Id = 1,
        //     UserName = "Leczo",
        //     NormalizedUserName = "LECZO",
        //     Email = "eee@eeee.ee",
        //     NormalizedEmail = "EEE@EEEE.EE",
        //     PhoneNumber = "123456789",
        //     DataUrodzenia = new DateOnly(2002, 10, 05),
        //     EmailConfirmed = true,
        //     SecurityStamp = "00000000-0000-0000-0000-000000000001",
        //     ConcurrencyStamp = "00000000-0000-0000-0000-000000000002",
        //     // aA1!12345 (podobno, chyba, mam nadzieję)
        //     PasswordHash = "AQAAAAIAAYagAAAAEP/9zHapujLFF7cdZeFgdRHL2ueNgRrIRoFWCoyV9eycnY+o/q5y7krDeXOf/7FWkA=="
        // });
        //
        // modelBuilder.Entity<Profil>().HasData(
        //     new Profil
        //     {
        //         IdUzytkownika = 1, Zaimki = "she/her", Opis = "Lubię placki!",
        //         Awatar = [], Pseudonim = "Leczo", RegionId = 2, StatusId = 1
        //     });

        // Zmiany nazw tabel Identity, aby było spójnie
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UzytkownikEFConfig).Assembly);
    }
}
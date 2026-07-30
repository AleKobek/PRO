using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Statystyki.Repositories;
using Squadra.Server.Modules.WspieraneGry.Models;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Drużyny.Models;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class StatystykiRepositoryTests
{
    private AppDbContext CreateContext()
    {

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void CzySpelniaWymagania_BasicChecks()
    {
        // Arrange
        var wymagania = new List<WartoscStatystykiDTO>
        {
            new WartoscStatystykiDTO(1, "", 10),
            new WartoscStatystykiDTO(53, "", 5) // 53 is checked inverted in repository
        };

        var statystykiDoSprawdzenia = new List<WartoscStatystykiDTO>
        {
            new WartoscStatystykiDTO(1, "", 15),
            new WartoscStatystykiDTO(53, "", 3) // for inverted check 3 <= 5 -> OK
        };

        using var ctx = CreateContext();
        var repo = new StatystykiRepository(ctx);

        // Act
        var result = repo.CzySpelniaWymagania(wymagania, statystykiDoSprawdzenia);

        // Assert
        Assert.True(result);

        // negative case: fail when one stat is too low
        statystykiDoSprawdzenia[0] = new WartoscStatystykiDTO(1, "", 5);
        var result2 = repo.CzySpelniaWymagania(wymagania, statystykiDoSprawdzenia);
        Assert.False(result2);
    }

    [Fact]
    public async Task CzyUzytkownikSpelniaOgolneWymaganiaDruzyny_UserMeetsRequirements_ReturnsTrue()
    {
        // Arrange
        using var ctx = CreateContext();
        // user
        ctx.Uzytkownik.Add(new Uzytkownik { Id = 1, UserName = "user1", NormalizedUserName = "USER1", Email = "user1@test.com", NormalizedEmail = "USER1@TEST.COM" });
        // druzyna
        ctx.Druzyna.Add(new Druzyna { Id = 1, Nazwa = "Team", GraId = 1, KapitanId = 1, CzyPubliczna = true, NastrojRozgrywkiId = 1 });
        // wspierana gra and category
        ctx.WspieranaGra.Add(new WspieranaGra { Id = 1, Tytul = "Game", Gatunek = "Action" });
        ctx.Kategoria.Add(new Kategoria { Id = 1, Nazwa = "General", IdGry = 1 });
        // statystyka required by team
        ctx.Statystyka.Add(new Statystyka { Id = 10, Nazwa = "Kills", KategoriaId = 1, CzyToCzasRozgrywki = false });
        ctx.WymaganaStatystykaDruzyny.Add(new WymaganaStatystykaDruzyny { DruzynaId = 1, StatystykaId = 10, Wartosc = "10", PorownywalnaWartoscLiczbowa = 10 });
        // user's stat meeting requirement
        ctx.StatystykaUzytkownika.Add(new StatystykaUzytkownika { UzytkownikId = 1, StatystykaId = 10, Wartosc = "15", PorownywalnaWartoscLiczbowa = 15 });

        await ctx.SaveChangesAsync();

        var repo = new StatystykiRepository(ctx);

        // Act
        var result = await repo.CzyUzytkownikSpelniaOgolneWymaganiaDruzyny(1, 1);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task CzyUzytkownikSpelniaOgolneWymaganiaDruzyny_UserDoesNotMeetRequirements_ReturnsFalse()
    {
        // Arrange
        using var ctx = CreateContext();
        // user
        ctx.Uzytkownik.Add(new Uzytkownik { Id = 1, UserName = "user1", NormalizedUserName = "USER1", Email = "user1@test.com", NormalizedEmail = "USER1@TEST.COM" });
        // druzyna
        ctx.Druzyna.Add(new Druzyna { Id = 1, Nazwa = "Team", GraId = 1, KapitanId = 1, CzyPubliczna = true, NastrojRozgrywkiId = 1 });
        // wspierana gra and category
        ctx.WspieranaGra.Add(new WspieranaGra { Id = 1, Tytul = "Game", Gatunek = "Action" });
        ctx.Kategoria.Add(new Kategoria { Id = 1, Nazwa = "General", IdGry = 1 });
        // statystyka required by team
        ctx.Statystyka.Add(new Statystyka { Id = 10, Nazwa = "Kills", KategoriaId = 1, CzyToCzasRozgrywki = false });
        ctx.WymaganaStatystykaDruzyny.Add(new WymaganaStatystykaDruzyny { DruzynaId = 1, StatystykaId = 10, Wartosc = "10", PorownywalnaWartoscLiczbowa = 10 });
        // user's stat not meeting requirement
        ctx.StatystykaUzytkownika.Add(new StatystykaUzytkownika { UzytkownikId = 1, StatystykaId = 10, Wartosc = "5", PorownywalnaWartoscLiczbowa = 5 });

        await ctx.SaveChangesAsync();

        var repo = new StatystykiRepository(ctx);

        // Act
        var result = await repo.CzyUzytkownikSpelniaOgolneWymaganiaDruzyny(1, 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetMniejszeLubRowneRangiGryUzytkownika_ReturnsExpected()
    {
        // Arrange
        using var ctx = CreateContext();
        // game, category, stat
        ctx.WspieranaGra.Add(new WspieranaGra { Id = 1, Tytul = "Game", Gatunek = "Action" });
        ctx.Kategoria.Add(new Kategoria { Id = 1, Nazwa = "General", IdGry = 1 });
        var stat = new Statystyka { Id = 1, Nazwa = "Ranked", KategoriaId = 1, CzyToCzasRozgrywki = false };
        ctx.Statystyka.Add(stat);
        // ranges
        ctx.Ranga.Add(new Ranga { StatystykaId = 1, Nazwa = "Bronze", WartoscLiczbowa = 10 });
        ctx.Ranga.Add(new Ranga { StatystykaId = 1, Nazwa = "Silver", WartoscLiczbowa = 20 });
        // user and user's stat (value between Bronze and Silver)
                ctx.Uzytkownik.Add(new Uzytkownik { Id = 1, UserName = "user1", NormalizedUserName = "USER1", Email = "user1@test.com", NormalizedEmail = "USER1@TEST.COM" });
        ctx.StatystykaUzytkownika.Add(new StatystykaUzytkownika { UzytkownikId = 1, StatystykaId = 1, Wartosc = "15", PorownywalnaWartoscLiczbowa = 15 });

        await ctx.SaveChangesAsync();

        var repo = new StatystykiRepository(ctx);

        // Act
        var result = await repo.GetMniejszeLubRowneRangiGryUzytkownika(1, 1);

        // Assert
        Assert.NotNull(result);
        var statDto = Assert.Single(result);
        Assert.Equal(1, statDto.Id);
        Assert.Single(statDto.Rangi);
        Assert.Equal("Bronze", statDto.Rangi.First().NazwaRangi);
        Assert.Equal(10, statDto.Rangi.First().WartoscLiczbowa);
    }
}

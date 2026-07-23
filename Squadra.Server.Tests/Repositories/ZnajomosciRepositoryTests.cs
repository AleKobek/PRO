using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Wiadomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Models;
using Squadra.Server.Modules.Znajomosci.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class ZnajomosciRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IProfileRepository> _mockProfilRepository;
    private readonly ZnajomosciRepository _repository;

    public ZnajomosciRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new AppDbContext(options);
        _mockProfilRepository = new Mock<IProfileRepository>();
        _repository = new ZnajomosciRepository(
            _context,
            _mockProfilRepository.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Znajomi.AddRange(
            new Znajomi
            {
                IdUzytkownika1 = 1,
                IdUzytkownika2 = 2,
                DataNawiazaniaZnajomosci = DateOnly.FromDateTime(DateTime.Now.AddDays(-10))
            },
            new Znajomi
            {
                IdUzytkownika1 = 1,
                IdUzytkownika2 = 3,
                DataNawiazaniaZnajomosci = DateOnly.FromDateTime(DateTime.Now.AddDays(-5))
            },
            new Znajomi
            {
                IdUzytkownika1 = 4,
                IdUzytkownika2 = 5,
                DataNawiazaniaZnajomosci = DateOnly.FromDateTime(DateTime.Now)
            }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_ReturnsUserFriends()
    {
        // Arrange
        var userId =1;

        // Potrzebne przez GetZnajomiUzytkownika: sprawdzenie istnienia użytkownika w tabeli Uzytkownik if (!await _context.Uzytkownik.AnyAsync(u => u.Id ==1))
        {
            _context.Uzytkownik.AddRange(
                new Uzytkownik { Id =1, UserName = "user1", Email = "eeee.eee@eeee.eee", NormalizedEmail = "eeee.eee@eeee.eee", NormalizedUserName = "user1"},
                new Uzytkownik { Id =2, UserName = "user2", Email = "eeee.eee@eee2e.eee", NormalizedEmail = "eeee.eee@eee2e.eee", NormalizedUserName = "user2"},
                new Uzytkownik { Id =3, UserName = "user3", Email = "eeee.eee@eeee.e2e", NormalizedEmail = "eeee.eee@eeee.e2e", NormalizedUserName = "user3" },
                new Uzytkownik { Id =4, UserName = "user4", Email = "eeee.eee@eee1e.eee", NormalizedEmail = "eeee.eee@eee1e.eee", NormalizedUserName = "user4"},
                new Uzytkownik { Id =5, UserName = "user5", Email = "eeee.eee@e1eee.eee", NormalizedEmail = "eeee.eee@e1eee.eee", NormalizedUserName = "user5"}
            );
            await _context.SaveChangesAsync();
        }

        // Act
        var result = await _repository.GetZnajomosciUzytkownika(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, z => z.IdUzytkownika2 ==2);
        Assert.Contains(result, z => z.IdUzytkownika2 ==3);
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_WithNoFriends_ReturnsEmptyList()
    {
        // Arrange
        _context.Uzytkownik.AddRange(
            new Uzytkownik { Id = 999, UserName = "user1", Email = "eeee.eee@eeee.eee", NormalizedEmail = "eeee.eee@eeee.eee", NormalizedUserName = "user1"}
        );
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetZnajomosciUzytkownika(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateZnajomosc_WithValidUsers_Createsfriendship()
    {
        // Arrange
        var userId1 = 10;
        var userId2 = 11;
        var profile1 = new ProfilGetResDto("User10", null, null, null, new List<JezykOrazStopienDto>(), null, "Active");
        var profile2 = new ProfilGetResDto("User11", null, null, null, new List<JezykOrazStopienDto>(), null, "Active");
        
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(userId1))
            .ReturnsAsync(profile1);
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(userId2))
            .ReturnsAsync(profile2);

        // Act
        var result = await _repository.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.True(result);
        var friendship = await _context.Znajomi
            .FirstOrDefaultAsync(z => z.IdUzytkownika1 == userId1 && z.IdUzytkownika2 == userId2);
        Assert.NotNull(friendship);
    }

    [Fact]
    public async Task CreateZnajomosc_WhenUserNotFound_ThrowsException()
    {
        // Arrange
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(It.IsAny<int>()))
            .ThrowsAsync(new NieZnalezionoWBazieException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.CreateZnajomosc(999, 1000));
    }

    [Fact]
    public async Task CzyJestZnajomosc_WithExistingFriendship_ReturnsTrue()
    {
        // Act
        var result = await _repository.CzyJestZnajomosc(1, 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CzyJestZnajomosc_WithReversedOrder_ReturnsTrue()
    {
        // Act - checking in reverse order
        var result = await _repository.CzyJestZnajomosc(2, 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CzyJestZnajomosc_WhenNotFriends_ReturnsFalse()
    {
        // Act
        var result = await _repository.CzyJestZnajomosc(1, 999);

        // Assert
        Assert.False(result);
    }
}

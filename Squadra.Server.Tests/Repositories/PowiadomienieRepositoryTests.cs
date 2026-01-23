using Microsoft.EntityFrameworkCore;
using Moq;
using Squadra.Server.Context;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class PowiadomienieRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IProfilRepository> _mockProfilRepository;
    private readonly PowiadomienieRepository _repository;

    public PowiadomienieRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockProfilRepository = new Mock<IProfilRepository>();
        _repository = new PowiadomienieRepository(_context, _mockProfilRepository.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.TypPowiadomienia.AddRange(
            new TypPowiadomienia { Id = 1, Nazwa = "Systemowe" },
            new TypPowiadomienia { Id = 2, Nazwa = "Zaproszenie do znajomych" },
            new TypPowiadomienia { Id = 4, Nazwa = "Odrzucono zaproszenie" }
        );

        _context.Powiadomienie.AddRange(
            new Powiadomienie
            {
                Id = 1,
                TypPowiadomieniaId = 1,
                UzytkownikId = 1,
                PowiazanyObiektId = null,
                PowiazanyObiektNazwa = null,
                Tresc = "System message",
                DataWyslania = DateTime.Now
            },
            new Powiadomienie
            {
                Id = 2,
                TypPowiadomieniaId = 2,
                UzytkownikId = 1,
                PowiazanyObiektId = 2,
                PowiazanyObiektNazwa = "User2",
                Tresc = null,
                DataWyslania = DateTime.Now.AddMinutes(-30)
            },
            new Powiadomienie
            {
                Id = 3,
                TypPowiadomieniaId = 2,
                UzytkownikId = 2,
                PowiazanyObiektId = 3,
                PowiazanyObiektNazwa = "User3",
                Tresc = null,
                DataWyslania = DateTime.Now.AddHours(-1)
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
    public async Task GetPowiadomienie_SystemNotification_ReturnsNotification()
    {
        // Act
        var result = await _repository.GetPowiadomienie(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.IdTypuPowiadomienia);
        Assert.Equal("System message", result.Tresc);
        Assert.Null(result.IdPowiazanegoObiektu);
    }

    [Fact]
    public async Task GetPowiadomienie_FriendInvitation_ReturnsWithProfileInfo()
    {
        // Arrange
        var profile = new ProfilGetResDto("User2", null, null, null, new List<JezykOrazStopienDto>(), null, "Active");
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(2))
            .ReturnsAsync(profile);

        // Act
        var result = await _repository.GetPowiadomienie(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal(2, result.IdTypuPowiadomienia);
        Assert.Equal(2, result.IdPowiazanegoObiektu);
        Assert.Equal("User2", result.NazwaPowiazanegoObiektu);
    }

    [Fact]
    public async Task GetPowiadomienie_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetPowiadomienie(999));
    }

    [Fact]
    public async Task GetPowiadomieniaUzytkownika_ReturnsUserNotifications()
    {
        // Arrange
        var profile2 = new ProfilGetResDto("User2", null, null, null, new List<JezykOrazStopienDto>(), null, "Active");
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(2))
            .ReturnsAsync(profile2);

        // Act
        var result = await _repository.GetPowiadomieniaUzytkownika(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetPowiadomieniaUzytkownika_WithNoNotifications_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetPowiadomieniaUzytkownika(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreatePowiadomienie_WithValidData_CreatesNotification()
    {
        // Arrange
        var dto = new PowiadomienieCreateDto(1, 5, null, null, "New system notification");

        // Act
        var result = await _repository.CreatePowiadomienie(dto);

        // Assert
        Assert.True(result);
        var notification = await _context.Powiadomienie
            .FirstOrDefaultAsync(p => p.UzytkownikId == 5 && p.Tresc == "New system notification");
        Assert.NotNull(notification);
        Assert.Equal(1, notification.TypPowiadomieniaId);
    }

    [Fact]
    public async Task DeletePowiadomienie_WithValidId_DeletesNotification()
    {
        // Act
        var result = await _repository.DeletePowiadomienie(2);

        // Assert
        Assert.True(result);
        var notification = await _context.Powiadomienie.FindAsync(2);
        Assert.Null(notification);
    }

    [Fact]
    public async Task DeletePowiadomienie_WithId1_ThrowsException()
    {
        // Act & Assert - special case: can't delete notification with ID 1
        var exception = await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.DeletePowiadomienie(1));
        Assert.Contains("Ello", exception.Message);
    }

    [Fact]
    public async Task DeletePowiadomienie_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.DeletePowiadomienie(999));
    }

    [Fact]
    public async Task DeletePowiadomieniaUzytkownika_RemovesAllUserNotifications()
    {
        // Arrange
        var profile2 = new ProfilGetResDto("User2", null, null, null, new List<JezykOrazStopienDto>(), null, "Active");
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(2))
            .ReturnsAsync(profile2);

        var initialCount = await _context.Powiadomienie.CountAsync(p => p.UzytkownikId == 1);
        Assert.Equal(2, initialCount);

        // Act
        var result = await _repository.DeletePowiadomieniaUzytkownika(1);

        // Assert
        Assert.True(result);
        var remainingCount = await _context.Powiadomienie.CountAsync(p => p.UzytkownikId == 1);
        Assert.Equal(0, remainingCount);
    }

    [Fact]
    public async Task GetNazwaTypuPowiadomienia_WithValidId_ReturnsName()
    {
        // Act
        var result = await _repository.GetNazwaTypuPowiadomienia(1);

        // Assert
        Assert.Equal("Systemowe", result);
    }

    [Fact]
    public async Task GetNazwaTypuPowiadomienia_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetNazwaTypuPowiadomienia(999));
    }
}

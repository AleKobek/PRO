using Moq;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Wiadomosci.Services;
using Squadra.Server.Modules.Znajomosci.DTO;
using Squadra.Server.Modules.Znajomosci.Models;
using Squadra.Server.Modules.Znajomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Services;

namespace Squadra.Server.Tests.Services;

public class ZnajomiServiceTests
{
    private readonly Mock<IZnajomiRepository> _mockRepository;
    private readonly Mock<IProfilService> _mockProfilService;
    private readonly Mock<IStatystykiCzatuService> _mockStatystykiCzatuService;
    private readonly ZnajomiService _service;

    public ZnajomiServiceTests()
    {
        _mockRepository = new Mock<IZnajomiRepository>();
        _mockProfilService = new Mock<IProfilService>();
        _mockStatystykiCzatuService = new Mock<IStatystykiCzatuService>();

        _service = new ZnajomiService(
            _mockRepository.Object,
            _mockProfilService.Object,
            _mockStatystykiCzatuService.Object);
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_WithValidId_ReturnsOkWithFriendsList()
    {
        var userId = 5;
        var expectedFriends = new List<Znajomi>
        {
            new() { IdUzytkownika1 = userId, IdUzytkownika2 = 11 },
            new() { IdUzytkownika1 = userId, IdUzytkownika2 = 12 }
        };

        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId))
            .ReturnsAsync(expectedFriends);

        var result = await _service.GetZnajomiUzytkownika(userId);

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        _mockRepository.Verify(r => r.GetZnajomiUzytkownika(userId), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task GetZnajomiUzytkownika_WithInvalidId_ReturnsNotFound(int userId)
    {
        var result = await _service.GetZnajomiUzytkownika(userId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.GetZnajomiUzytkownika(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        var userId = 999;
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Uzytkownik nie istnieje"));

        var result = await _service.GetZnajomiUzytkownika(userId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetZnajomiDoListyUzytkownika_WithValidId_ReturnsOk()
    {
        var userId = 1;
        var znajomi = new List<Znajomi>
        {
            new()
            {
                IdUzytkownika1 = userId,
                IdUzytkownika2 = 2,
                OstatnieOtwarcieCzatuUzytkownika1 = DateTime.UtcNow.AddHours(-2)
            }
        };

        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId)).ReturnsAsync(znajomi);

        _mockProfilService
            .Setup(s => s.GetProfil(2))
            .ReturnsAsync(ServiceResult<Modules.Profile.DTO.Profil.ProfilGetResDto>.Ok(
                new Modules.Profile.DTO.Profil.ProfilGetResDto("Znajomy", null, null, null, new List<Modules.Profile.DTO.JezykStopien.JezykOrazStopienDto>(), null, "Online")));

        _mockStatystykiCzatuService
            .Setup(s => s.GetDataNajnowszejWiadomosci(userId, 2))
            .ReturnsAsync(ServiceResult<DateTime?>.Ok(DateTime.UtcNow.AddMinutes(-5)));

        _mockStatystykiCzatuService
            .Setup(s => s.CzySaNoweWiadomosciOdZnajomego(userId, 2))
            .ReturnsAsync(ServiceResult<bool>.Ok(true));

        var result = await _service.GetZnajomiDoListyUzytkownika(userId);

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.IsType<ZnajomyDoListyDto>(result.Value.First());
    }

    [Fact]
    public async Task CreateZnajomosc_WithValidIds_ReturnsCreated()
    {
        var userId1 = 1;
        var userId2 = 2;

        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId1))
            .ReturnsAsync(new List<Znajomi>());
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId2))
            .ReturnsAsync(new List<Znajomi>());
        _mockRepository.Setup(r => r.CreateZnajomosc(userId1, userId2))
            .ReturnsAsync(true);

        var result = await _service.CreateZnajomosc(userId1, userId2);

        Assert.True(result.Succeeded);
        Assert.Equal(201, result.StatusCode);
        Assert.True(result.Value);
        _mockRepository.Verify(r => r.CreateZnajomosc(userId1, userId2), Times.Once);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 0)]
    public async Task CreateZnajomosc_WithInvalidIds_ReturnsNotFound(int userId1, int userId2)
    {
        var result = await _service.CreateZnajomosc(userId1, userId2);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CreateZnajomosc_WithSameIds_ReturnsBadRequest()
    {
        var userId = 5;

        var result = await _service.CreateZnajomosc(userId, userId);

        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task DeleteZnajomosc_WithValidIds_ReturnsNoContent()
    {
        var userId1 = 1;
        var userId2 = 2;

        _mockRepository.Setup(r => r.DeleteZnajomosc(userId1, userId2))
            .ReturnsAsync(true);

        var result = await _service.DeleteZnajomosc(userId1, userId2);

        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        Assert.True(result.Value);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, -1)]
    public async Task DeleteZnajomosc_WithInvalidIds_ReturnsNotFound(int userId1, int userId2)
    {
        var result = await _service.DeleteZnajomosc(userId1, userId2);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }
}

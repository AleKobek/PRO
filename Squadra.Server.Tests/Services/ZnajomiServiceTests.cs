using Moq;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class ZnajomiServiceTests
{
    private readonly Mock<IZnajomiRepository> _mockRepository;
    private readonly ZnajomiService _service;

    public ZnajomiServiceTests()
    {
        _mockRepository = new Mock<IZnajomiRepository>();
        _service = new ZnajomiService(_mockRepository.Object);
    }

    #region GetZnajomiUzytkownika Tests

    [Fact]
    public async Task GetZnajomiUzytkownika_WithValidId_ReturnsOkWithFriendsList()
    {
        // Arrange
        var userId = 5;
        var expectedFriends = new List<ProfilGetResDto>
        {
            new ProfilGetResDto("Friend1", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active"),
            new ProfilGetResDto("Friend2", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active")
        };
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId))
            .ReturnsAsync(expectedFriends);

        // Act
        var result = await _service.GetZnajomiUzytkownika(userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        _mockRepository.Verify(r => r.GetZnajomiUzytkownika(userId), Times.Once);
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId = 0;

        // Act
        var result = await _service.GetZnajomiUzytkownika(userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
        _mockRepository.Verify(r => r.GetZnajomiUzytkownika(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_WithNegativeId_ReturnsNotFound()
    {
        // Arrange
        var userId = -5;

        // Act
        var result = await _service.GetZnajomiUzytkownika(userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.GetZnajomiUzytkownika(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetZnajomiUzytkownika_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.GetZnajomiUzytkownika(userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    #endregion

    #region CreateZnajomosc Tests

    [Fact]
    public async Task CreateZnajomosc_WithValidIds_ReturnsCreated()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId1))
            .ReturnsAsync(new List<ProfilGetResDto>());
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId2))
            .ReturnsAsync(new List<ProfilGetResDto>());
        _mockRepository.Setup(r => r.CreateZnajomosc(userId1, userId2))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(201, result.StatusCode);
        Assert.True(result.Value);
        _mockRepository.Verify(r => r.CreateZnajomosc(userId1, userId2), Times.Once);
    }

    [Fact]
    public async Task CreateZnajomosc_WithFirstIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 0;
        var userId2 = 2;

        // Act
        var result = await _service.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateZnajomosc_WithSecondIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 0;

        // Act
        var result = await _service.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateZnajomosc_WithSameIds_ReturnsBadRequest()
    {
        // Arrange
        var userId = 5;

        // Act
        var result = await _service.CreateZnajomosc(userId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("nie może dodać siebie", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateZnajomosc_WhenUser1HasMaxFriends_ReturnsBadRequest()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        var maxFriends = Enumerable.Range(1, ZnajomiService.MaxLiczbaZnajomych)
            .Select(i => new ProfilGetResDto($"Friend{i}", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active"))
            .ToList();

        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId1))
            .ReturnsAsync(maxFriends);

        // Act
        var result = await _service.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("maksymalną liczbę znajomych", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateZnajomosc_WhenUser2HasMaxFriends_ReturnsBadRequest()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        var maxFriends = Enumerable.Range(1, ZnajomiService.MaxLiczbaZnajomych)
            .Select(i => new ProfilGetResDto($"Friend{i}", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active"))
            .ToList();

        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId1))
            .ReturnsAsync(new List<ProfilGetResDto>());
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId2))
            .ReturnsAsync(maxFriends);

        // Act
        var result = await _service.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("maksymalną liczbę znajomych", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateZnajomosc_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 999;
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId1))
            .ReturnsAsync(new List<ProfilGetResDto>());
        _mockRepository.Setup(r => r.GetZnajomiUzytkownika(userId2))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.CreateZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region DeleteZnajomosc Tests

    [Fact]
    public async Task DeleteZnajomosc_WithValidIds_ReturnsNoContent()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        _mockRepository.Setup(r => r.DeleteZnajomosc(userId1, userId2))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteZnajomosc(userId1, userId2);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        Assert.True(result.Value);
        _mockRepository.Verify(r => r.DeleteZnajomosc(userId1, userId2), Times.Once);
    }

    [Fact]
    public async Task DeleteZnajomosc_WithFirstIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 0;
        var userId2 = 2;

        // Act
        var result = await _service.DeleteZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteZnajomosc_WithSecondIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = -1;

        // Act
        var result = await _service.DeleteZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteZnajomosc_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 999;
        _mockRepository.Setup(r => r.DeleteZnajomosc(userId1, userId2))
            .ThrowsAsync(new NieZnalezionoWBazieException("Relacja nie istnieje"));

        // Act
        var result = await _service.DeleteZnajomosc(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    #endregion
}

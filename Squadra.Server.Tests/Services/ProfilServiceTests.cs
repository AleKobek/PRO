using Moq;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class ProfilServiceTests
{
    private readonly Mock<IProfilRepository> _mockProfilRepository;
    private readonly Mock<IUzytkownikRepository> _mockUzytkownikRepository;
    private readonly Mock<IStatusRepository> _mockStatusRepository;
    private readonly ProfilService _service;

    public ProfilServiceTests()
    {
        _mockProfilRepository = new Mock<IProfilRepository>();
        _mockUzytkownikRepository = new Mock<IUzytkownikRepository>();
        _mockStatusRepository = new Mock<IStatusRepository>();
        _service = new ProfilService(
            _mockProfilRepository.Object,
            _mockUzytkownikRepository.Object,
            _mockStatusRepository.Object);
    }

    #region GetProfil Tests

    [Fact]
    public async Task GetProfil_ById_WithValidId_ReturnsOk()
    {
        // Arrange
        var profileId = 1;
        var expectedProfile = new ProfilGetResDto(
            "TestUser",
            null,
            "they/them",
            "Test description",
            new List<JezykOrazStopienDto>(),
            null,
            "Online"
        );
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(profileId))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _service.GetProfil(profileId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("TestUser", result.Value.Pseudonim);
        _mockProfilRepository.Verify(r => r.GetProfilUzytkownika(profileId), Times.Once);
    }

    [Fact]
    public async Task GetProfil_ById_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var profileId = 0;

        // Act
        var result = await _service.GetProfil(profileId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
        _mockProfilRepository.Verify(r => r.GetProfilUzytkownika(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetProfil_ById_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var profileId = 999;
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(profileId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Profil nie istnieje"));

        // Act
        var result = await _service.GetProfil(profileId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetProfil_ByLogin_WithValidLogin_ReturnsOk()
    {
        // Arrange
        var login = "testuser";
        var userId = 1;
        var expectedUser = new UzytkownikResDto(userId, login, "test@test.com", "123456789", new DateOnly(1990, 1, 1), new[] { "User" });
        var expectedProfile = new ProfilGetResDto(
            "TestUser",
            null,
            null,
            null,
            new List<JezykOrazStopienDto>(),
            null,
            "Online"
        );
        _mockUzytkownikRepository.Setup(r => r.GetUzytkownik(login))
            .ReturnsAsync(expectedUser);
        _mockProfilRepository.Setup(r => r.GetProfilUzytkownika(userId))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _service.GetProfil(login);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetProfil_ByLogin_WithEmptyLogin_ReturnsNotFound()
    {
        // Arrange
        var login = "";

        // Act
        var result = await _service.GetProfil(login);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("login", result.Errors[0].Message.ToLower());
    }

    [Fact]
    public async Task GetProfil_ByLogin_WithWhitespaceLogin_ReturnsNotFound()
    {
        // Arrange
        var login = "   ";

        // Act
        var result = await _service.GetProfil(login);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetProfil_ByLogin_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var login = "nonexistent";
        _mockUzytkownikRepository.Setup(r => r.GetUzytkownik(login))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.GetProfil(login);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region UpdateProfil Tests

    [Fact]
    public async Task UpdateProfil_WithIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var profileId = 0;
        var dto = new ProfilUpdateDto(1, "they/them", "Test description", new List<JezykProfiluCreateDto>(), "TestUser");

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        _mockProfilRepository.Verify(r => r.UpdateProfil(It.IsAny<int>(), It.IsAny<ProfilUpdateDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfil_WithRegionIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var profileId = 1;
        var dto = new ProfilUpdateDto(0, "they/them", "Test description", new List<JezykProfiluCreateDto>(), "TestUser");

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Region", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateProfil_WithZaimkiTooLong_ReturnsBadRequest()
    {
        // Arrange
        var profileId = 1;
        var dto = new ProfilUpdateDto(1, "they/them/extremely/long", "Test description", new List<JezykProfiluCreateDto>(), "TestUser");

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("zaimków", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateProfil_WithOpisTooLong_ReturnsBadRequest()
    {
        // Arrange
        var profileId = 1;
        var longDescription = new string('a', 101); // 101 characters
        var dto = new ProfilUpdateDto(1, "they/them", longDescription, new List<JezykProfiluCreateDto>(), "TestUser");

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("opisu", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateProfil_WithPseudonimTooLong_ReturnsBadRequest()
    {
        // Arrange
        var profileId = 1;
        var longPseudonym = new string('a', 21); // 21 characters
        var dto = new ProfilUpdateDto(1, "they/them", "Test description", new List<JezykProfiluCreateDto>(), longPseudonym);

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("pseudonimu", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateProfil_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var profileId = 1;
        var dto = new ProfilUpdateDto(1, "they/them", "Test description", new List<JezykProfiluCreateDto>(), "TestUser");
        _mockProfilRepository.Setup(r => r.UpdateProfil(profileId, dto))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        _mockProfilRepository.Verify(r => r.UpdateProfil(profileId, dto), Times.Once);
    }

    [Fact]
    public async Task UpdateProfil_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var profileId = 999;
        var dto = new ProfilUpdateDto(1, "they/them", "Test description", new List<JezykProfiluCreateDto>(), "TestUser");
        _mockProfilRepository.Setup(r => r.UpdateProfil(profileId, dto))
            .ThrowsAsync(new NieZnalezionoWBazieException("Profil nie istnieje"));

        // Act
        var result = await _service.UpdateProfil(profileId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}

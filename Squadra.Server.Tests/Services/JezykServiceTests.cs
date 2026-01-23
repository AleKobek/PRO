using Moq;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class JezykServiceTests
{
    private readonly Mock<IJezykRepository> _mockRepository;
    private readonly JezykService _service;

    public JezykServiceTests()
    {
        _mockRepository = new Mock<IJezykRepository>();
        _service = new JezykService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetJezyki_ReturnsOkWithLanguagesList()
    {
        // Arrange
        var expectedLanguages = new List<JezykDto>
        {
            new JezykDto(1, "English"),
            new JezykDto(2, "Polish"),
            new JezykDto(3, "German")
        };
        _mockRepository.Setup(r => r.GetJezyki())
            .ReturnsAsync(expectedLanguages);

        // Act
        var result = await _service.GetJezyki();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
        _mockRepository.Verify(r => r.GetJezyki(), Times.Once);
    }

    [Fact]
    public async Task GetJezyk_WithValidId_ReturnsOk()
    {
        // Arrange
        var languageId = 1;
        var expectedLanguage = new JezykDto(languageId, "English");
        _mockRepository.Setup(r => r.GetJezyk(languageId))
            .ReturnsAsync(expectedLanguage);

        // Act
        var result = await _service.GetJezyk(languageId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(languageId, result.Value.Id);
        _mockRepository.Verify(r => r.GetJezyk(languageId), Times.Once);
    }

    [Fact]
    public async Task GetJezyk_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var languageId = 0;

        // Act
        var result = await _service.GetJezyk(languageId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
        _mockRepository.Verify(r => r.GetJezyk(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetJezyk_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var languageId = 999;
        _mockRepository.Setup(r => r.GetJezyk(languageId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Język nie istnieje"));

        // Act
        var result = await _service.GetJezyk(languageId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetJezykiProfilu_WithValidId_ReturnsOk()
    {
        // Arrange
        var profileId = 1;
        var expectedLanguages = new List<JezykOrazStopienDto>
        {
            new JezykOrazStopienDto(new JezykDto(1, "English"), new StopienBieglosciJezykaDto(3, "Advanced", 3)),
            new JezykOrazStopienDto(new JezykDto(2, "Polish"), new StopienBieglosciJezykaDto(5, "Native", 5))
        };
        _mockRepository.Setup(r => r.GetJezykiProfilu(profileId))
            .ReturnsAsync(expectedLanguages);

        // Act
        var result = await _service.GetJezykiProfilu(profileId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        _mockRepository.Verify(r => r.GetJezykiProfilu(profileId), Times.Once);
    }

    [Fact]
    public async Task GetJezykiProfilu_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var profileId = 0;

        // Act
        var result = await _service.GetJezykiProfilu(profileId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.GetJezykiProfilu(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetJezykiProfilu_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var profileId = 999;
        _mockRepository.Setup(r => r.GetJezykiProfilu(profileId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Profil nie istnieje"));

        // Act
        var result = await _service.GetJezykiProfilu(profileId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task ZmienJezykiProfilu_WithValidData_ReturnsOk()
    {
        // Arrange
        var profileId = 1;
        var newLanguages = new List<JezykProfiluCreateDto>
        {
            new JezykProfiluCreateDto(1, 3),
            new JezykProfiluCreateDto(2, 5)
        };
        var expectedResult = new List<JezykOrazStopienDto>
        {
            new JezykOrazStopienDto(new JezykDto(1, "English"), new StopienBieglosciJezykaDto(3, "Advanced", 3)),
            new JezykOrazStopienDto(new JezykDto(2, "Polish"), new StopienBieglosciJezykaDto(5, "Native", 5))
        };
        _mockRepository.Setup(r => r.ZmienJezykiProfilu(profileId, newLanguages))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ZmienJezykiProfilu(profileId, newLanguages);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        _mockRepository.Verify(r => r.ZmienJezykiProfilu(profileId, newLanguages), Times.Once);
    }

    [Fact]
    public async Task ZmienJezykiProfilu_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var profileId = 0;
        var newLanguages = new List<JezykProfiluCreateDto>();

        // Act
        var result = await _service.ZmienJezykiProfilu(profileId, newLanguages);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.ZmienJezykiProfilu(It.IsAny<int>(), It.IsAny<ICollection<JezykProfiluCreateDto>>()), Times.Never);
    }

    [Fact]
    public async Task ZmienJezykiProfilu_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var profileId = 999;
        var newLanguages = new List<JezykProfiluCreateDto>();
        _mockRepository.Setup(r => r.ZmienJezykiProfilu(profileId, newLanguages))
            .ThrowsAsync(new NieZnalezionoWBazieException("Profil nie istnieje"));

        // Act
        var result = await _service.ZmienJezykiProfilu(profileId, newLanguages);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }
}

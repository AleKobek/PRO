using Moq;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class KrajServiceTests
{
    private readonly Mock<IKrajRepository> _mockRepository;
    private readonly KrajService _service;

    public KrajServiceTests()
    {
        _mockRepository = new Mock<IKrajRepository>();
        _service = new KrajService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetKraje_ReturnsOkWithCountriesList()
    {
        // Arrange
        var expectedCountries = new List<KrajDto>
        {
            new KrajDto(1, "Poland"),
            new KrajDto(2, "Germany"),
            new KrajDto(3, "France")
        };
        _mockRepository.Setup(r => r.GetKraje())
            .ReturnsAsync(expectedCountries);

        // Act
        var result = await _service.GetKraje();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
        _mockRepository.Verify(r => r.GetKraje(), Times.Once);
    }

    [Fact]
    public async Task GetKraj_WithValidId_ReturnsOk()
    {
        // Arrange
        var countryId = 1;
        var expectedCountry = new KrajDto(countryId, "Poland");
        _mockRepository.Setup(r => r.GetKraj(countryId))
            .ReturnsAsync(expectedCountry);

        // Act
        var result = await _service.GetKraj(countryId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(countryId, result.Value.Id);
        _mockRepository.Verify(r => r.GetKraj(countryId), Times.Once);
    }

    [Fact]
    public async Task GetKraj_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var countryId = 0;

        // Act
        var result = await _service.GetKraj(countryId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
        _mockRepository.Verify(r => r.GetKraj(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetKraj_WithNegativeId_ReturnsNotFound()
    {
        // Arrange
        var countryId = -5;

        // Act
        var result = await _service.GetKraj(countryId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.GetKraj(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetKraj_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var countryId = 999;
        _mockRepository.Setup(r => r.GetKraj(countryId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Kraj nie istnieje"));

        // Act
        var result = await _service.GetKraj(countryId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }
}

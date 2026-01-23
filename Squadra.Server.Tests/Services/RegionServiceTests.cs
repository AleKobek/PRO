using Moq;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class RegionServiceTests
{
    private readonly Mock<IRegionRepository> _mockRepository;
    private readonly RegionService _service;

    public RegionServiceTests()
    {
        _mockRepository = new Mock<IRegionRepository>();
        _service = new RegionService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetRegiony_ReturnsOkWithRegionsList()
    {
        // Arrange
        var expectedRegions = new List<RegionDto>
        {
            new RegionDto(1, 1, "Mazowieckie"),
            new RegionDto(2, 1, "Małopolskie"),
            new RegionDto(3, 2, "Bavaria")
        };
        _mockRepository.Setup(r => r.GetRegiony())
            .ReturnsAsync(expectedRegions);

        // Act
        var result = await _service.GetRegiony();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
        _mockRepository.Verify(r => r.GetRegiony(), Times.Once);
    }

    [Fact]
    public async Task GetRegion_WithValidId_ReturnsOk()
    {
        // Arrange
        var regionId = 1;
        var expectedRegion = new RegionDto(regionId, 1, "Mazowieckie");
        _mockRepository.Setup(r => r.GetRegion(regionId))
            .ReturnsAsync(expectedRegion);

        // Act
        var result = await _service.GetRegion(regionId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(regionId, result.Value.Id);
        _mockRepository.Verify(r => r.GetRegion(regionId), Times.Once);
    }

    [Fact]
    public async Task GetRegion_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var regionId = 0;

        // Act
        var result = await _service.GetRegion(regionId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
        _mockRepository.Verify(r => r.GetRegion(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetRegion_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var regionId = 999;
        _mockRepository.Setup(r => r.GetRegion(regionId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Region nie istnieje"));

        // Act
        var result = await _service.GetRegion(regionId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetRegionIKraj_WithValidId_ReturnsOk()
    {
        // Arrange
        var regionId = 1;
        var expectedRegionKraj = new RegionKrajDto(regionId, "Mazowieckie", 1, "Poland");
        _mockRepository.Setup(r => r.GetRegionIKraj(regionId))
            .ReturnsAsync(expectedRegionKraj);

        // Act
        var result = await _service.GetRegionIKraj(regionId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(regionId, result.Value.IdRegionu);
        _mockRepository.Verify(r => r.GetRegionIKraj(regionId), Times.Once);
    }

    [Fact]
    public async Task GetRegionIKraj_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var regionId = 0;

        // Act
        var result = await _service.GetRegionIKraj(regionId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.GetRegionIKraj(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetRegionIKraj_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var regionId = 999;
        _mockRepository.Setup(r => r.GetRegionIKraj(regionId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Region nie istnieje"));

        // Act
        var result = await _service.GetRegionIKraj(regionId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetRegionyKraju_WithValidCountryId_ReturnsOk()
    {
        // Arrange
        var countryId = 1;
        var expectedRegions = new List<RegionDto>
        {
            new RegionDto(1, countryId, "Mazowieckie"),
            new RegionDto(2, countryId, "Małopolskie")
        };
        _mockRepository.Setup(r => r.GetRegionyKraju(countryId))
            .ReturnsAsync(expectedRegions);

        // Act
        var result = await _service.GetRegionyKraju(countryId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        _mockRepository.Verify(r => r.GetRegionyKraju(countryId), Times.Once);
    }

    [Fact]
    public async Task GetRegionyKraju_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var countryId = 999;
        _mockRepository.Setup(r => r.GetRegionyKraju(countryId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Kraj nie istnieje"));

        // Act
        var result = await _service.GetRegionyKraju(countryId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }
}

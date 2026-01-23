using Moq;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class StopienBieglosciJezykaServiceTests
{
    private readonly Mock<IStopienBieglosciJezykaRepository> _mockRepository;
    private readonly StopienBieglosciJezykaService _service;

    public StopienBieglosciJezykaServiceTests()
    {
        _mockRepository = new Mock<IStopienBieglosciJezykaRepository>();
        _service = new StopienBieglosciJezykaService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetStopnieBieglosciJezyka_ReturnsOkWithProficiencyLevelsList()
    {
        // Arrange
        var expectedLevels = new List<StopienBieglosciJezykaDto>
        {
            new StopienBieglosciJezykaDto(1, "Beginner", 1),
            new StopienBieglosciJezykaDto(2, "Intermediate", 2),
            new StopienBieglosciJezykaDto(3, "Advanced", 3),
            new StopienBieglosciJezykaDto(4, "Fluent", 4),
            new StopienBieglosciJezykaDto(5, "Native", 5)
        };
        _mockRepository.Setup(r => r.GetStopnieBieglosciJezyka())
            .ReturnsAsync(expectedLevels);

        // Act
        var result = await _service.GetStopnieBieglosciJezyka();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Count);
        _mockRepository.Verify(r => r.GetStopnieBieglosciJezyka(), Times.Once);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithValidId_ReturnsOk()
    {
        // Arrange
        var levelId = 3;
        var expectedLevel = new StopienBieglosciJezykaDto(levelId, "Advanced", 3);
        _mockRepository.Setup(r => r.GetStopienBieglosciJezyka(levelId))
            .ReturnsAsync(expectedLevel);

        // Act
        var result = await _service.GetStopienBieglosciJezyka(levelId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(levelId, result.Value.Id);
        Assert.Equal("Advanced", result.Value.Nazwa);
        _mockRepository.Verify(r => r.GetStopienBieglosciJezyka(levelId), Times.Once);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var levelId = 0;

        // Act
        var result = await _service.GetStopienBieglosciJezyka(levelId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
        _mockRepository.Verify(r => r.GetStopienBieglosciJezyka(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithNegativeId_ReturnsNotFound()
    {
        // Arrange
        var levelId = -5;

        // Act
        var result = await _service.GetStopienBieglosciJezyka(levelId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        _mockRepository.Verify(r => r.GetStopienBieglosciJezyka(It.IsAny<int>()), Times.Never);
    }
}

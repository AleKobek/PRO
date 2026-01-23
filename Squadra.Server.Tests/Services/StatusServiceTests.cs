using Moq;
using Squadra.Server.DTO.Status;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class StatusServiceTests
{
    private readonly Mock<IStatusRepository> _mockRepository;
    private readonly StatusService _service;

    public StatusServiceTests()
    {
        _mockRepository = new Mock<IStatusRepository>();
        _service = new StatusService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetStatusy_ReturnsOkWithStatusList()
    {
        // Arrange
        var expectedStatuses = new List<StatusDto>
        {
            new StatusDto(1, "Online"),
            new StatusDto(2, "Offline"),
            new StatusDto(3, "Busy")
        };
        _mockRepository.Setup(r => r.GetStatusy())
            .ReturnsAsync(expectedStatuses);

        // Act
        var result = await _service.GetStatusy();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
        _mockRepository.Verify(r => r.GetStatusy(), Times.Once);
    }

    [Fact]
    public async Task GetStatus_WithValidId_ReturnsOk()
    {
        // Arrange
        var statusId = 1;
        var expectedStatus = new StatusDto(statusId, "Online");
        _mockRepository.Setup(r => r.GetStatus(statusId))
            .ReturnsAsync(expectedStatus);

        // Act
        var result = await _service.GetStatus(statusId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(statusId, result.Value.Id);
        _mockRepository.Verify(r => r.GetStatus(statusId), Times.Once);
    }

    [Fact]
    public async Task GetStatus_WhenStatusNotFound_ReturnsNotFound()
    {
        // Arrange
        var statusId = 999;
        _mockRepository.Setup(r => r.GetStatus(statusId))
            .ReturnsAsync((StatusDto?)null);

        // Act
        var result = await _service.GetStatus(statusId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public void GetStatusDomyslny_ReturnsDefaultStatus()
    {
        // Arrange
        var defaultStatus = new StatusDto(2, "Offline");
        _mockRepository.Setup(r => r.GetStatusOffline())
            .Returns(defaultStatus);

        // Act
        var result = _service.GetStatusDomyslny();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Offline", result.Nazwa);
        _mockRepository.Verify(r => r.GetStatusOffline(), Times.Once);
    }
}

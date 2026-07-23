using Moq;
using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Profile.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class StatusyServiceTests
{
    private readonly Mock<IStatusyRepository> _mockRepository;
    private readonly StatusyService _service;

    public StatusyServiceTests()
    {
        _mockRepository = new Mock<IStatusyRepository>();
        _service = new StatusyService(_mockRepository.Object);
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
    }
}

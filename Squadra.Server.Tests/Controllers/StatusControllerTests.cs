using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.Status;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class StatusControllerTests
{
    private readonly Mock<IStatusService> _mockStatusService;
    private readonly StatusController _controller;

    public StatusControllerTests()
    {
        _mockStatusService = new Mock<IStatusService>();
        _controller = new StatusController(_mockStatusService.Object);
    }

    [Fact]
    public async Task GetStatusy_ReturnsOkWithStatusList()
    {
        // Arrange
        ICollection<StatusDto> statuses = new List<StatusDto>
        {
            new StatusDto(1, "Online"),
            new StatusDto(2, "Offline"),
            new StatusDto(3, "Busy")
        };
        var result = ServiceResult<ICollection<StatusDto>>.Ok(statuses);
        _mockStatusService.Setup(s => s.GetStatusy())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStatusy();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedStatuses = Assert.IsAssignableFrom<IEnumerable<StatusDto>>(okResult.Value);
        Assert.Equal(3, returnedStatuses.Count());
    }

    [Fact]
    public async Task GetStatus_WithValidId_ReturnsOkWithStatus()
    {
        // Arrange
        var statusDto = new StatusDto(1, "Online");
        var result = ServiceResult<StatusDto?>.Ok(statusDto);
        _mockStatusService.Setup(s => s.GetStatus(1))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStatus(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedStatus = Assert.IsType<StatusDto>(okResult.Value);
        Assert.Equal("Online", returnedStatus.Nazwa);
    }

    [Fact]
    public async Task GetStatus_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<StatusDto?>.Fail(404, 
            new[] { new ErrorItem("Status not found", "id") });
        _mockStatusService.Setup(s => s.GetStatus(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStatus(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Status not found", notFoundResult.Value);
    }
}

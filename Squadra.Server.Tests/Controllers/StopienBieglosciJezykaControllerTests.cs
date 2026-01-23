using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class StopienBieglosciJezykaControllerTests
{
    private readonly Mock<IStopienBieglosciJezykaService> _mockStopienBieglosciJezykaService;
    private readonly StopienBieglosciJezykaController _controller;

    public StopienBieglosciJezykaControllerTests()
    {
        _mockStopienBieglosciJezykaService = new Mock<IStopienBieglosciJezykaService>();
        _controller = new StopienBieglosciJezykaController(_mockStopienBieglosciJezykaService.Object);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_ReturnsOkWithProficiencyLevelList()
    {
        // Arrange
        ICollection<StopienBieglosciJezykaDto> proficiencyLevels = new List<StopienBieglosciJezykaDto>
        {
            new StopienBieglosciJezykaDto(1, "Beginner", 1),
            new StopienBieglosciJezykaDto(2, "Elementary", 2),
            new StopienBieglosciJezykaDto(3, "Intermediate", 3),
            new StopienBieglosciJezykaDto(4, "Advanced", 4),
            new StopienBieglosciJezykaDto(5, "Native", 5)
        };
        var result = ServiceResult<ICollection<StopienBieglosciJezykaDto>>.Ok(proficiencyLevels);
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopnieBieglosciJezyka())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLevels = Assert.IsAssignableFrom<IEnumerable<StopienBieglosciJezykaDto>>(okResult.Value);
        Assert.Equal(5, returnedLevels.Count());
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithValidId_ReturnsOkWithProficiencyLevel()
    {
        // Arrange
        var proficiencyLevelDto = new StopienBieglosciJezykaDto(3, "Intermediate", 3);
        var result = ServiceResult<StopienBieglosciJezykaDto?>.Ok(proficiencyLevelDto);
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopienBieglosciJezyka(3))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka(3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLevel = Assert.IsType<StopienBieglosciJezykaDto>(okResult.Value);
        Assert.Equal("Intermediate", returnedLevel.Nazwa);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<StopienBieglosciJezykaDto?>.Fail(404, 
            new[] { new ErrorItem("Proficiency level not found", "id") });
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopienBieglosciJezyka(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Proficiency level not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_EmptyList_ReturnsOkWithEmptyCollection()
    {
        // Arrange
        ICollection<StopienBieglosciJezykaDto> emptyList = new List<StopienBieglosciJezykaDto>();
        var result = ServiceResult<ICollection<StopienBieglosciJezykaDto>>.Ok(emptyList);
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopnieBieglosciJezyka())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLevels = Assert.IsAssignableFrom<IEnumerable<StopienBieglosciJezykaDto>>(okResult.Value);
        Assert.Empty(returnedLevels);
    }
}

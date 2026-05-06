using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Modules.Profile.Controllers;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
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
        ICollection<StopienBieglosciJezyka> proficiencyLevels = new List<StopienBieglosciJezyka>
        {
            new StopienBieglosciJezyka { Id = 1, Nazwa = "Beginner", Wartosc = 1},
            new StopienBieglosciJezyka { Id = 2, Nazwa = "Elementary", Wartosc = 2 },
            new StopienBieglosciJezyka { Id = 3, Nazwa = "Intermediate", Wartosc = 3},
            new StopienBieglosciJezyka { Id = 4, Nazwa = "Advanced", Wartosc = 4},
            new StopienBieglosciJezyka { Id = 5, Nazwa = "Native", Wartosc = 5}
        };
        var result = ServiceResult<ICollection<StopienBieglosciJezyka>>.Ok(proficiencyLevels);
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopnieBieglosciJezyka())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLevels = Assert.IsAssignableFrom<IEnumerable<StopienBieglosciJezyka>>(okResult.Value);
        Assert.Equal(5, returnedLevels.Count());
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithValidId_ReturnsOkWithProficiencyLevel()
    {
        // Arrange
        var proficiencyLevelDto = new StopienBieglosciJezyka { Id = 3, Nazwa = "Intermediate", Wartosc = 3};
        var result = ServiceResult<StopienBieglosciJezyka?>.Ok(proficiencyLevelDto);
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopienBieglosciJezyka(3))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka(3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLevel = Assert.IsType<StopienBieglosciJezyka>(okResult.Value);
        Assert.Equal("Intermediate", returnedLevel.Nazwa);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<StopienBieglosciJezyka>.Fail(404, 
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
        ICollection<StopienBieglosciJezyka> emptyList = new List<StopienBieglosciJezyka>();
        var result = ServiceResult<ICollection<StopienBieglosciJezyka>>.Ok(emptyList);
        _mockStopienBieglosciJezykaService.Setup(s => s.GetStopnieBieglosciJezyka())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetStopienBieglosciJezyka();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLevels = Assert.IsAssignableFrom<IEnumerable<StopienBieglosciJezyka>>(okResult.Value);
        Assert.Empty(returnedLevels);
    }
}

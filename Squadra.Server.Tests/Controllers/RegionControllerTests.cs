using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class RegionControllerTests
{
    private readonly Mock<IRegionService> _mockRegionService;
    private readonly RegionController _controller;

    public RegionControllerTests()
    {
        _mockRegionService = new Mock<IRegionService>();
        _controller = new RegionController(_mockRegionService.Object);
    }

    [Fact]
    public async Task GetRegiony_ReturnsOkWithRegionList()
    {
        // Arrange
        ICollection<RegionDto> regions = new List<RegionDto>
        {
            new RegionDto(1, 1, "Mazowieckie"),
            new RegionDto(2, 1, "Wielkopolskie"),
            new RegionDto(3, 1, "Śląskie")
        };
        var result = ServiceResult<ICollection<RegionDto>>.Ok(regions);
        _mockRegionService.Setup(s => s.GetRegiony())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetRegiony();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedRegions = Assert.IsAssignableFrom<IEnumerable<RegionDto>>(okResult.Value);
        Assert.Equal(3, returnedRegions.Count());
    }

    [Fact]
    public async Task GetRegion_WithValidId_ReturnsOkWithRegion()
    {
        // Arrange
        var regionDto = new RegionDto(1, 1, "Mazowieckie");
        var result = ServiceResult<RegionDto?>.Ok(regionDto);
        _mockRegionService.Setup(s => s.GetRegion(1))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetRegion(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedRegion = Assert.IsType<RegionDto>(okResult.Value);
        Assert.Equal("Mazowieckie", returnedRegion.Nazwa);
    }

    [Fact]
    public async Task GetRegion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<RegionDto?>.Fail(404, 
            new[] { new ErrorItem("Region not found", "id") });
        _mockRegionService.Setup(s => s.GetRegion(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetRegion(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Region not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetRegionyKraju_WithValidId_ReturnsOkWithRegions()
    {
        // Arrange
        var krajId = 1;
        ICollection<RegionDto> regions = new List<RegionDto>
        {
            new RegionDto(1, krajId, "Mazowieckie"),
            new RegionDto(2, krajId, "Wielkopolskie")
        };
        var result = ServiceResult<ICollection<RegionDto>>.Ok(regions);
        _mockRegionService.Setup(s => s.GetRegionyKraju(krajId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetRegionyKraju(krajId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedRegions = Assert.IsAssignableFrom<IEnumerable<RegionDto>>(okResult.Value);
        Assert.Equal(2, returnedRegions.Count());
    }

    [Fact]
    public async Task GetRegionyKraju_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<ICollection<RegionDto>>.Fail(404, 
            new[] { new ErrorItem("Country not found", "id") });
        _mockRegionService.Setup(s => s.GetRegionyKraju(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetRegionyKraju(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Country not found", notFoundResult.Value);
    }
}

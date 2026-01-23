using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class KrajControllerTests
{
    private readonly Mock<IKrajService> _mockKrajService;
    private readonly KrajController _controller;

    public KrajControllerTests()
    {
        _mockKrajService = new Mock<IKrajService>();
        _controller = new KrajController(_mockKrajService.Object);
    }

    [Fact]
    public async Task GetKraje_ReturnsOkWithCountryList()
    {
        // Arrange
        ICollection<KrajDto> countries = new List<KrajDto>
        {
            new KrajDto(1, "Poland"),
            new KrajDto(2, "Germany"),
            new KrajDto(3, "France")
        };
        var result = ServiceResult<ICollection<KrajDto>>.Ok(countries);
        _mockKrajService.Setup(s => s.GetKraje())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetKraje();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedCountries = Assert.IsAssignableFrom<IEnumerable<KrajDto>>(okResult.Value);
        Assert.Equal(3, returnedCountries.Count());
    }

    [Fact]
    public async Task GetKraj_WithValidId_ReturnsOkWithCountry()
    {
        // Arrange
        var countryDto = new KrajDto(1, "Poland");
        var result = ServiceResult<KrajDto?>.Ok(countryDto);
        _mockKrajService.Setup(s => s.GetKraj(1))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetKraj(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedCountry = Assert.IsType<KrajDto>(okResult.Value);
        Assert.Equal("Poland", returnedCountry.Nazwa);
    }

    [Fact]
    public async Task GetKraj_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<KrajDto?>.Fail(404, 
            new[] { new ErrorItem("Country not found", "id") });
        _mockKrajService.Setup(s => s.GetKraj(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetKraj(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Country not found", notFoundResult.Value);
    }
}

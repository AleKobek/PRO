using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class JezykControllerTests
{
    private readonly Mock<IJezykService> _mockJezykService;
    private readonly JezykController _controller;

    public JezykControllerTests()
    {
        _mockJezykService = new Mock<IJezykService>();
        _controller = new JezykController(_mockJezykService.Object);
    }

    [Fact]
    public async Task GetJezyki_ReturnsOkWithLanguageList()
    {
        // Arrange
        ICollection<JezykDto> languages = new List<JezykDto>
        {
            new JezykDto(1, "English"),
            new JezykDto(2, "Polish"),
            new JezykDto(3, "German")
        };
        var result = ServiceResult<ICollection<JezykDto>>.Ok(languages);
        _mockJezykService.Setup(s => s.GetJezyki())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetJezyki();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLanguages = Assert.IsAssignableFrom<IEnumerable<JezykDto>>(okResult.Value);
        Assert.Equal(3, returnedLanguages.Count());
    }

    [Fact]
    public async Task GetJezyk_WithValidId_ReturnsOkWithLanguage()
    {
        // Arrange
        var languageDto = new JezykDto(1, "English");
        var result = ServiceResult<JezykDto?>.Ok(languageDto);
        _mockJezykService.Setup(s => s.GetJezyk(1))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetJezyk(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLanguage = Assert.IsType<JezykDto>(okResult.Value);
        Assert.Equal("English", returnedLanguage.Nazwa);
    }

    [Fact]
    public async Task GetJezyk_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<JezykDto?>.Fail(404, 
            new[] { new ErrorItem("Language not found", "id") });
        _mockJezykService.Setup(s => s.GetJezyk(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetJezyk(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Language not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetJezykiProfilu_WithValidId_ReturnsOkWithLanguages()
    {
        // Arrange
        var profilId = 1;
        ICollection<JezykOrazStopienDto> languages = new List<JezykOrazStopienDto>
        {
            new JezykOrazStopienDto(
                new JezykDto(1, "English"),
                new StopienBieglosciJezykaDto(3, "Advanced", 3)
            ),
            new JezykOrazStopienDto(
                new JezykDto(2, "Polish"),
                new StopienBieglosciJezykaDto(5, "Native", 5)
            )
        };
        var result = ServiceResult<ICollection<JezykOrazStopienDto>>.Ok(languages);
        _mockJezykService.Setup(s => s.GetJezykiProfilu(profilId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetJezykiProfilu(profilId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLanguages = Assert.IsAssignableFrom<IEnumerable<JezykOrazStopienDto>>(okResult.Value);
        Assert.Equal(2, returnedLanguages.Count());
    }

    [Fact]
    public async Task GetJezykiProfilu_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<ICollection<JezykOrazStopienDto>>.Fail(404, 
            new[] { new ErrorItem("Profile not found", "id") });
        _mockJezykService.Setup(s => s.GetJezykiProfilu(999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetJezykiProfilu(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }
}

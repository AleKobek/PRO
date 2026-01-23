using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.Wiadomosc;
using Squadra.Server.Models;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class WiadomoscControllerTests
{
    private readonly Mock<IWiadomoscService> _mockWiadomoscService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly WiadomoscController _controller;

    public WiadomoscControllerTests()
    {
        _mockWiadomoscService = new Mock<IWiadomoscService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _controller = new WiadomoscController(_mockWiadomoscService.Object, _mockUserManager.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetWiadomosc_WithValidId_ReturnsOkWithMessage()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var messageDto = new WiadomoscDto(1, 2, DateTime.Now, "Hello!", 1);
        var result = ServiceResult<WiadomoscDto>.Ok(messageDto);
        _mockWiadomoscService.Setup(s => s.GetWiadomosc(1, user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetWiadomosc(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedMessage = Assert.IsType<WiadomoscDto>(okResult.Value);
        Assert.Equal("Hello!", returnedMessage.Tresc);
    }

    [Fact]
    public async Task GetWiadomosc_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetWiadomosc(1);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetWiadomosc_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<WiadomoscDto>.Fail(404, 
            new[] { new ErrorItem("Message not found", "id") });
        _mockWiadomoscService.Setup(s => s.GetWiadomosc(999, user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetWiadomosc(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Message not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetWiadomosc_WhenForbidden_ReturnsForbidden()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<WiadomoscDto>.Fail(403, 
            new[] { new ErrorItem("Access forbidden", "id") });
        _mockWiadomoscService.Setup(s => s.GetWiadomosc(1, user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetWiadomosc(1);

        // Assert
        var forbiddenResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, forbiddenResult.StatusCode);
        Assert.Equal("Access forbidden", forbiddenResult.Value);
    }

    [Fact]
    public async Task GetWiadomosci_WithValidRecipientId_ReturnsOkWithMessages()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        ICollection<WiadomoscDto> messages = new List<WiadomoscDto>
        {
            new WiadomoscDto(1, 2, DateTime.Now, "Hello!", 1),
            new WiadomoscDto(2, 1, DateTime.Now.AddMinutes(1), "Hi there!", 1)
        };
        var result = ServiceResult<ICollection<WiadomoscDto>>.Ok(messages);
        _mockWiadomoscService.Setup(s => s.GetWiadomosci(user.Id, 2))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetWiadomosci(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedMessages = Assert.IsAssignableFrom<IEnumerable<WiadomoscDto>>(okResult.Value);
        Assert.Equal(2, returnedMessages.Count());
    }

    [Fact]
    public async Task GetWiadomosci_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetWiadomosci(2);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task CreateWiadomosc_WithValidData_ReturnsCreated()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var createDto = new WiadomoscCreateDto(2, "Hello!", 1);
        var result = ServiceResult<bool>.Ok(true, 201);
        _mockWiadomoscService.Setup(s => s.CreateWiadomosc(createDto, user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.CreateWiadomosc(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(actionResult);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task CreateWiadomosc_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var createDto = new WiadomoscCreateDto(2, "Hello!", 1);

        // Act
        var actionResult = await _controller.CreateWiadomosc(createDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task CreateWiadomosc_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var createDto = new WiadomoscCreateDto(2, "", 1);
        var result = ServiceResult<bool>.Fail(400, 
            new[] { new ErrorItem("Message content is required", "Tresc") });
        _mockWiadomoscService.Setup(s => s.CreateWiadomosc(createDto, user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.CreateWiadomosc(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Message content is required", badRequestResult.Value);
    }
}

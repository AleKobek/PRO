using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Modules.Powiadomienia.Controllers;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class PowiadomieniaControllerTests
{
    private readonly Mock<IPowiadomieniaService> _mockPowiadomienieService;
    private readonly Mock<IRozpatrzPowiadomienieService> _mockRozpatrzPowiadomienieService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly PowiadomieniaController _controller;

    public PowiadomieniaControllerTests()
    {
        _mockPowiadomienieService = new Mock<IPowiadomieniaService>();
        _mockRozpatrzPowiadomienieService = new Mock<IRozpatrzPowiadomienieService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _controller = new PowiadomieniaController(_mockPowiadomienieService.Object,_mockRozpatrzPowiadomienieService.Object , _mockUserManager.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetPowiadomienia_ReturnsOkWithNotifications()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var notifications = new List<PowiadomienieDto>
        {
            new PowiadomienieDto(1, 1, 1, null, null, null, null, "Notification 1", DateTime.Now.ToString("dd.MM.yyyy HH:mm")),
            new PowiadomienieDto(2, 1, 1, null, null, null, null, "Notification 2", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
        };
        var result = ServiceResult<ICollection<PowiadomienieDto>>.Ok(notifications);
        _mockPowiadomienieService.Setup(s => s.GetPowiadomieniaUzytkownika(1)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetPowiadomienia();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedNotifications = Assert.IsAssignableFrom<IEnumerable<PowiadomienieDto>>(okResult.Value);
        Assert.Equal(2, returnedNotifications.Count());
    }

    [Fact]
    public async Task GetPowiadomienia_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetPowiadomienia();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }
    
    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WithValidLogin_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<bool>.Ok(true, 204);
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomychPoLoginie(1, "friend"))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomychPoLoginie("friend");

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomychPoLoginie("friend");

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<bool>.Fail(404, 
            new[] { new ErrorItem("User not found", "loginZapraszanegoUzytkownika") });
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomychPoLoginie(1, "nonexistent"))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomychPoLoginie("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenConflict_ReturnsConflict()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<bool>.Fail(409, 
            new[] { new ErrorItem("Invitation already sent", null) });
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomychPoLoginie(1, "friend"))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomychPoLoginie("friend");

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult);
        Assert.Equal("Invitation already sent", conflictResult.Value);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<bool>.Fail(400, 
            new[] { new ErrorItem("Invalid data", "loginZapraszanegoUzytkownika") });
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomychPoLoginie(1, ""))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomychPoLoginie("");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Invalid data", badRequestResult.Value);
    }
}

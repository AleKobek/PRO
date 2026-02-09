using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.Models;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class PowiadomienieControllerTests
{
    private readonly Mock<IPowiadomienieService> _mockPowiadomienieService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly PowiadomienieController _controller;

    public PowiadomienieControllerTests()
    {
        _mockPowiadomienieService = new Mock<IPowiadomienieService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _controller = new PowiadomienieController(_mockPowiadomienieService.Object, _mockUserManager.Object);
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
            new PowiadomienieDto(1, 1, 1, null, null, "Notification 1", DateTime.Now.ToString("dd.MM.yyyy HH:mm")),
            new PowiadomienieDto(2, 1, 1, null, null, "Notification 2", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
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
    public async Task GetPowiadomienie_WithValidId_ReturnsOkWithNotification()
    {
        // Arrange
        var notification = new PowiadomienieDto(1, 1, 1, null, null, "Notification", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        var result = ServiceResult<PowiadomienieDto>.Ok(notification);
        _mockPowiadomienieService.Setup(s => s.GetPowiadomienie(1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(result);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetPowiadomienie(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedNotification = Assert.IsType<PowiadomienieDto>(okResult.Value);
        Assert.Equal("Notification", returnedNotification.Tresc);
    }

    [Fact]
    public async Task GetPowiadomienie_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<PowiadomienieDto>.Fail(404, 
            new[] { new ErrorItem("Notification not found", "id") });
        _mockPowiadomienieService.Setup(s => s.GetPowiadomienie(999, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(result);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetPowiadomienie(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Notification not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetPowiadomienie_WhenUnauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var result = ServiceResult<PowiadomienieDto>.Fail(401, 
            new[] { new ErrorItem("Nie jesteś zalogowany.", null) });
        _mockPowiadomienieService.Setup(s => s.GetPowiadomienie(1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(result);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetPowiadomienie(1);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetPowiadomienie_WhenForbidden_ReturnsForbidden()
    {
        // Arrange
        var result = ServiceResult<PowiadomienieDto>.Fail(403, 
            new[] { new ErrorItem("Access forbidden", null) });
        _mockPowiadomienieService.Setup(s => s.GetPowiadomienie(1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(result);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetPowiadomienie(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        Assert.Equal("Access forbidden", statusResult.Value);
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
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomych(1, "friend"))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomych("friend");

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
        var actionResult = await _controller.WyslijZaproszenieDoZnajomych("friend");

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
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomych(1, "nonexistent"))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomych("nonexistent");

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
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomych(1, "friend"))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomych("friend");

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
        _mockPowiadomienieService.Setup(s => s.WyslijZaproszenieDoZnajomych(1, ""))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.WyslijZaproszenieDoZnajomych("");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Invalid data", badRequestResult.Value);
    }
}

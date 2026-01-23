using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Models;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class UzytkownikControllerTests
{
    private readonly Mock<IUzytkownikService> _mockUzytkownikService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly UzytkownikController _controller;

    public UzytkownikControllerTests()
    {
        _mockUzytkownikService = new Mock<IUzytkownikService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _controller = new UzytkownikController(_mockUzytkownikService.Object, _mockUserManager.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetUzytkownicy_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<UzytkownikResDto>
        {
            new UzytkownikResDto(1, "user1", "user1@test.com", null, null, new string[] { "User" }),
            new UzytkownikResDto(2, "user2", "user2@test.com", null, null, new string[] { "User" })
        };
        var result = ServiceResult<ICollection<UzytkownikResDto>>.Ok(users);
        _mockUzytkownikService.Setup(s => s.GetUzytkownicy()).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetUzytkownicy();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UzytkownikResDto>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count());
    }

    [Fact]
    public async Task GetUzytkownikById_WithValidId_ReturnsOkWithUser()
    {
        // Arrange
        var user = new UzytkownikResDto(1, "user1", "user1@test.com", null, null, new string[] { "User" });
        var result = ServiceResult<UzytkownikResDto>.Ok(user);
        _mockUzytkownikService.Setup(s => s.GetUzytkownik(1)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetUzytkownikById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUser = Assert.IsType<UzytkownikResDto>(okResult.Value);
        Assert.Equal("user1", returnedUser.Login);
    }

    [Fact]
    public async Task GetUzytkownikById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<UzytkownikResDto>.Fail(404, 
            new[] { new ErrorItem("User not found", "id") });
        _mockUzytkownikService.Setup(s => s.GetUzytkownik(999)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetUzytkownikById(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateUzytkownik_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new UzytkownikUpdateDto("user1@test.com", "user1", null, new DateOnly(2000, 1, 1));
        var result = ServiceResult<bool>.Ok(true, 204);
        _mockUzytkownikService.Setup(s => s.UpdateUzytkownik(1, updateDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateUzytkownik(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateUzytkownik_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new UzytkownikUpdateDto("user1@test.com", "user1", null, new DateOnly(2000, 1, 1));

        // Act
        var actionResult = await _controller.UpdateUzytkownik(1, updateDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateUzytkownik_WhenUserIdMismatch_ReturnsForbidden()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new UzytkownikUpdateDto("user1@test.com", "user1", null, new DateOnly(2000, 1, 1));

        // Act
        var actionResult = await _controller.UpdateUzytkownik(2, updateDto);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        Assert.Equal("Nie możesz edytować konta innego użytkownika.", statusResult.Value);
    }

    [Fact]
    public async Task UpdateUzytkownik_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new UzytkownikUpdateDto("invalid-email", "user1", null, new DateOnly(2000, 1, 1));
        var result = ServiceResult<bool>.Fail(400, 
            new[] { new ErrorItem("Invalid email format", "email") });
        _mockUzytkownikService.Setup(s => s.UpdateUzytkownik(1, updateDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateUzytkownik(1, updateDto);

        // Assert
        Assert.IsAssignableFrom<IActionResult>(actionResult);
    }

    [Fact]
    public async Task UpdateUzytkownik_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new UzytkownikUpdateDto("user1@test.com", "user1", null, new DateOnly(2000, 1, 1));
        var result = ServiceResult<bool>.Fail(404, 
            new[] { new ErrorItem("User not found", "id") });
        _mockUzytkownikService.Setup(s => s.UpdateUzytkownik(1, updateDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateUzytkownik(1, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteUzytkownik_WithValidId_ReturnsNoContent()
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
        _mockUzytkownikService.Setup(s => s.DeleteUzytkownik(1)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteUzytkownik(1);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task DeleteUzytkownik_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.DeleteUzytkownik(1);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task DeleteUzytkownik_WhenUserIdMismatch_ReturnsForbidden()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.DeleteUzytkownik(2);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        Assert.Equal("Nie możesz usunąć konta innego użytkownika.", statusResult.Value);
    }

    [Fact]
    public async Task DeleteUzytkownik_WhenUserNotFound_ReturnsNotFound()
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
            new[] { new ErrorItem("User not found", "id") });
        _mockUzytkownikService.Setup(s => s.DeleteUzytkownik(1)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteUzytkownik(1);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
    }
}

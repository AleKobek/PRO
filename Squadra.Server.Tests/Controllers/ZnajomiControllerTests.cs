using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Models;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class ZnajomiControllerTests
{
    private readonly Mock<IZnajomiService> _mockZnajomiService;
    private readonly Mock<IPowiadomienieService> _mockPowiadomienieService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly Mock<IProfilService> _mockProfilService;
    private readonly ZnajomiController _controller;

    public ZnajomiControllerTests()
    {
        _mockZnajomiService = new Mock<IZnajomiService>();
        _mockPowiadomienieService = new Mock<IPowiadomienieService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _mockProfilService = new Mock<IProfilService>();
        _controller = new ZnajomiController(
            _mockZnajomiService.Object, 
            _mockPowiadomienieService.Object, 
            _mockUserManager.Object,
            _mockProfilService.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetZnajomi_ReturnsOkWithFriendsList()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        ICollection<ProfilGetResDto> friends = new List<ProfilGetResDto>
        {
            new ProfilGetResDto(
                "Friend1",
                new RegionKrajDto(1, "Mazowieckie", 1, "Poland"),
                "he/him",
                "Description 1",
                new List<JezykOrazStopienDto>(),
                null,
                "Online"
            ),
            new ProfilGetResDto(
                "Friend2",
                new RegionKrajDto(2, "Śląskie", 1, "Poland"),
                "she/her",
                "Description 2",
                new List<JezykOrazStopienDto>(),
                null,
                "Offline"
            )
        };
        var result = ServiceResult<ICollection<ProfilGetResDto>>.Ok(friends);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetZnajomi();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedFriends = Assert.IsAssignableFrom<IEnumerable<ProfilGetResDto>>(okResult.Value);
        Assert.Equal(2, returnedFriends.Count());
    }

    [Fact]
    public async Task GetZnajomi_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.GetZnajomi();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetZnajomi_WhenNoFriendsFound_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<ICollection<ProfilGetResDto>>.Fail(404, 
            new[] { new ErrorItem("No friends found", "userId") });
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(user.Id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetZnajomi();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("No friends found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteZnajomego_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var deleteResult = ServiceResult<bool>.Ok(true, 204);
        _mockZnajomiService.Setup(s => s.DeleteZnajomosc(user.Id, 2))
            .ReturnsAsync(deleteResult);

        var friendProfile = new ProfilGetResDto(
            "Friend1",
            new RegionKrajDto(1, "Mazowieckie", 1, "Poland"),
            "he/him",
            "Description",
            new List<JezykOrazStopienDto>(),
            null,
            "Online"
        );
        var profilResult = ServiceResult<ProfilGetResDto>.Ok(friendProfile);
        _mockProfilService.Setup(s => s.GetProfil(2))
            .ReturnsAsync(profilResult);

        var notificationResult = ServiceResult<bool>.Ok(true, 204);
        _mockPowiadomienieService.Setup(s => s.CreatePowiadomienie(It.IsAny<DTO.Powiadomienie.PowiadomienieCreateDto>()))
            .ReturnsAsync(notificationResult);

        // Act
        var actionResult = await _controller.DeleteZnajomego(2);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(actionResult.Result);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteZnajomego_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.DeleteZnajomego(2);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task DeleteZnajomego_WhenFriendshipNotFound_ReturnsNotFound()
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
            new[] { new ErrorItem("Friendship not found", "id") });
        _mockZnajomiService.Setup(s => s.DeleteZnajomosc(user.Id, 999))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteZnajomego(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Friendship not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteZnajomego_WhenProfileNotFound_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var deleteResult = ServiceResult<bool>.Ok(true, 204);
        _mockZnajomiService.Setup(s => s.DeleteZnajomosc(user.Id, 2))
            .ReturnsAsync(deleteResult);

        var profilResult = ServiceResult<ProfilGetResDto>.Fail(404, 
            new[] { new ErrorItem("Profile not found", "id") });
        _mockProfilService.Setup(s => s.GetProfil(2))
            .ReturnsAsync(profilResult);

        // Act
        var actionResult = await _controller.DeleteZnajomego(2);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }
}

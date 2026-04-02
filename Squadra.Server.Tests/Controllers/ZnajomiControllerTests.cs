using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Znajomosci.Controllers;
using Squadra.Server.Modules.Znajomosci.DTO;
using Squadra.Server.Modules.Znajomosci.Services;
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
        return new Mock<UserManager<TUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private void SetUserInHttpContext()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };
    }

    [Fact]
    public async Task GetZnajomiDoListy_ReturnsOkWithFriendsList()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        ICollection<ZnajomyDoListyDto> friends = new List<ZnajomyDoListyDto>
        {
            new(2, "Friend1", Array.Empty<byte>(), DateTime.UtcNow.AddMinutes(-5), "Online", true),
            new(3, "Friend2", Array.Empty<byte>(), DateTime.UtcNow.AddMinutes(-15), "Offline", false)
        };

        _mockZnajomiService
            .Setup(s => s.GetZnajomiDoListyUzytkownika(user.Id))
            .ReturnsAsync(ServiceResult<ICollection<ZnajomyDoListyDto>>.Ok(friends));

        var actionResult = await _controller.GetZnajomiDoListy();

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedFriends = Assert.IsAssignableFrom<ICollection<ZnajomyDoListyDto>>(okResult.Value);
        Assert.Equal(2, returnedFriends.Count);
    }

    [Fact]
    public async Task GetZnajomiDoListy_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        SetUserInHttpContext();

        var actionResult = await _controller.GetZnajomiDoListy();

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetZnajomiDoListy_WhenNoFriendsFound_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        _mockZnajomiService
            .Setup(s => s.GetZnajomiDoListyUzytkownika(user.Id))
            .ReturnsAsync(ServiceResult<ICollection<ZnajomyDoListyDto>>.Fail(
                404,
                new[] { new ErrorItem("No friends found", "userId") }));

        var actionResult = await _controller.GetZnajomiDoListy();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("No friends found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteZnajomego_WithValidId_ReturnsNoContent()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        _mockZnajomiService.Setup(s => s.DeleteZnajomosc(user.Id, 2))
            .ReturnsAsync(ServiceResult<bool>.Ok(true, 204));

        _mockProfilService.Setup(s => s.GetProfil(user.Id))
            .ReturnsAsync(ServiceResult<Squadra.Server.Modules.Profile.DTO.Profil.ProfilGetResDto>.Ok(
                new Squadra.Server.Modules.Profile.DTO.Profil.ProfilGetResDto(
                    "Tester",
                    null,
                    null,
                    null,
                    new List<Squadra.Server.Modules.Profile.DTO.JezykStopien.JezykOrazStopienDto>(),
                    null,
                    "Online")));

        _mockPowiadomienieService.Setup(s => s.CreatePowiadomienie(It.IsAny<PowiadomienieCreateDto>()))
            .ReturnsAsync(ServiceResult<bool>.Ok(true, 204));

        var actionResult = await _controller.DeleteZnajomego(2);

        Assert.IsType<NoContentResult>(actionResult.Result);
    }

    [Fact]
    public async Task DeleteZnajomego_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        SetUserInHttpContext();

        var actionResult = await _controller.DeleteZnajomego(2);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task DeleteZnajomego_WhenFriendshipNotFound_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        _mockZnajomiService.Setup(s => s.DeleteZnajomosc(user.Id, 999))
            .ReturnsAsync(ServiceResult<bool>.Fail(
                404,
                new[] { new ErrorItem("Friendship not found", "id") }));

        var actionResult = await _controller.DeleteZnajomego(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Friendship not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteZnajomego_WhenProfileNotFound_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        _mockZnajomiService.Setup(s => s.DeleteZnajomosc(user.Id, 2))
            .ReturnsAsync(ServiceResult<bool>.Ok(true, 204));

        _mockProfilService.Setup(s => s.GetProfil(user.Id))
            .ReturnsAsync(ServiceResult<Squadra.Server.Modules.Profile.DTO.Profil.ProfilGetResDto>.Fail(
                404,
                new[] { new ErrorItem("Profile not found", "id") }));

        var actionResult = await _controller.DeleteZnajomego(2);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task ZaktualizujOstatnieOtwarcieCzatu_WithValidId_ReturnsNoContent()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        _mockZnajomiService.Setup(s => s.ZaktualizujOstatnieOtwarcieCzatu(user.Id, 2))
            .ReturnsAsync(ServiceResult<bool>.Ok(true, 204));

        var actionResult = await _controller.ZaktualizujOstatnieOtwarcieCzatu(2);

        Assert.IsType<NoContentResult>(actionResult.Result);
    }

    [Fact]
    public async Task ZaktualizujOstatnieOtwarcieCzatu_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        SetUserInHttpContext();

        var actionResult = await _controller.ZaktualizujOstatnieOtwarcieCzatu(2);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task ZaktualizujOstatnieOtwarcieCzatu_WhenFriendshipNotFound_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        SetUserInHttpContext();

        _mockZnajomiService.Setup(s => s.ZaktualizujOstatnieOtwarcieCzatu(user.Id, 999))
            .ReturnsAsync(ServiceResult<bool>.Fail(
                404,
                new[] { new ErrorItem("Friendship not found", "idZnajomego") }));

        var actionResult = await _controller.ZaktualizujOstatnieOtwarcieCzatu(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Friendship not found", notFoundResult.Value);
    }
}

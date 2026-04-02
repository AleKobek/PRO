using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Modules.Profile.Controllers;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.KrajRegion;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class ProfilControllerTests
{
    private readonly Mock<IProfilService> _mockProfilService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly ProfilController _controller;

    public ProfilControllerTests()
    {
        _mockProfilService = new Mock<IProfilService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _controller = new ProfilController(_mockProfilService.Object, _mockUserManager.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetProfile_ReturnsOkWithProfiles()
    {
        var profiles = new List<ProfilGetResDto>
        {
            new ProfilGetResDto("User1", new RegionKrajDto(1, "Mazowieckie", 1, "Poland"), "he/him", "Description 1", new List<JezykOrazStopienDto>(), Array.Empty<byte>(), "Online"),
            new ProfilGetResDto("User2", new RegionKrajDto(2, "Slaskie", 1, "Poland"), "she/her", "Description 2", new List<JezykOrazStopienDto>(), Array.Empty<byte>(), "Offline")
        };
        _mockProfilService.Setup(s => s.GetProfile()).ReturnsAsync(ServiceResult<ICollection<ProfilGetResDto>>.Ok(profiles));

        var actionResult = await _controller.GetProfile();

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProfiles = Assert.IsAssignableFrom<IEnumerable<ProfilGetResDto>>(okResult.Value);
        Assert.Equal(2, returnedProfiles.Count());
    }

    [Fact]
    public async Task GetProfil_WithValidId_ReturnsOkWithProfile()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var profile = new ProfilGetResDto("User1", new RegionKrajDto(1, "Mazowieckie", 1, "Poland"), "he/him", "Description", new List<JezykOrazStopienDto>(), Array.Empty<byte>(), "Online");
        _mockProfilService.Setup(s => s.GetProfil(1)).ReturnsAsync(ServiceResult<ProfilGetResDto>.Ok(profile));

        var actionResult = await _controller.GetProfil(1);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProfile = Assert.IsType<ProfilGetResDto>(okResult.Value);
        Assert.Equal("User1", returnedProfile.Pseudonim);
    }

    [Fact]
    public async Task GetProfil_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var actionResult = await _controller.GetProfil(1);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetProfil_WithInvalidId_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _mockProfilService.Setup(s => s.GetProfil(999)).ReturnsAsync(ServiceResult<ProfilGetResDto>.Fail(404, new[] { new ErrorItem("Profile not found", "id") }));

        var actionResult = await _controller.GetProfil(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateProfil_WithValidData_ReturnsNoContent()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var updateDto = new ProfilUpdateDto(1, "he/him", "Updated description", new List<JezykProfiluCreateDto>(), "TestUser");
        _mockProfilService.Setup(s => s.UpdateProfil(1, updateDto)).ReturnsAsync(ServiceResult<bool>.Ok(true, 204));

        var actionResult = await _controller.UpdateProfil(updateDto);

        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateProfil_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");
        var actionResult = await _controller.UpdateProfil(updateDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateProfil_WithInvalidData_ReturnsValidationProblem()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");
        _mockProfilService.Setup(s => s.UpdateProfil(1, updateDto)).ReturnsAsync(ServiceResult<bool>.Fail(400, new[] { new ErrorItem("Invalid data", "description") }));

        var actionResult = await _controller.UpdateProfil(updateDto);

        Assert.IsType<ObjectResult>(actionResult);
    }

    [Fact]
    public async Task UpdateProfil_WhenProfileNotFound_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");
        _mockProfilService.Setup(s => s.UpdateProfil(1, updateDto)).ReturnsAsync(ServiceResult<bool>.Fail(404, new[] { new ErrorItem("Profile not found", "id") }));

        var actionResult = await _controller.UpdateProfil(updateDto);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateAwatar_WithValidFile_ReturnsNoContent()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("avatar.jpg");
        mockFile.Setup(f => f.Length).Returns(1024);

        _mockProfilService.Setup(s => s.UpdateAwatar(1, mockFile.Object)).ReturnsAsync(ServiceResult<bool>.Ok(true, 204));

        var actionResult = await _controller.UpdateAwatar(mockFile.Object);

        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateAwatar_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var mockFile = new Mock<IFormFile>();
        var actionResult = await _controller.UpdateAwatar(mockFile.Object);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateAwatar_WithInvalidFile_ReturnsValidationProblem()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var mockFile = new Mock<IFormFile>();
        _mockProfilService.Setup(s => s.UpdateAwatar(1, mockFile.Object)).ReturnsAsync(ServiceResult<bool>.Fail(400, new[] { new ErrorItem("Invalid file format", "awatar") }));

        var actionResult = await _controller.UpdateAwatar(mockFile.Object);

        Assert.IsType<ObjectResult>(actionResult);
    }

    [Fact]
    public async Task UpdateStatus_WithValidStatus_ReturnsNoContent()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _mockProfilService.Setup(s => s.UpdateStatus(1, 2)).ReturnsAsync(ServiceResult<StatusDto>.Ok(new StatusDto(2, "Status Name"), 204));

        var actionResult = await _controller.UpdateStatus(2);

        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateStatus_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var actionResult = await _controller.UpdateStatus(2);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateStatus_WhenStatusNotFound_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _mockProfilService.Setup(s => s.UpdateStatus(1, 999)).ReturnsAsync(ServiceResult<StatusDto>.Fail(400, new[] { new ErrorItem("Status not found", "idStatus") }));

        var actionResult = await _controller.UpdateStatus(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Status not found", notFoundResult.Value);
    }
}

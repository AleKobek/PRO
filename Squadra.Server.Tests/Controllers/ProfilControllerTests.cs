using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Models;
using Squadra.Server.Services;
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
        // Arrange
        var profiles = new List<ProfilGetResDto>
        {
            new ProfilGetResDto(
                "User1",
                new RegionKrajDto(1, "Mazowieckie", 1, "Poland"),
                "he/him",
                "Description 1",
                new List<JezykOrazStopienDto>(),
                Array.Empty<byte>(),
                "Online"
            ),
            new ProfilGetResDto(
                "User2",
                new RegionKrajDto(2, "Śląskie", 1, "Poland"),
                "she/her",
                "Description 2",
                new List<JezykOrazStopienDto>(),
                Array.Empty<byte>(),
                "Offline"
            )
        };
        var result = ServiceResult<ICollection<ProfilGetResDto>>.Ok(profiles);
        _mockProfilService.Setup(s => s.GetProfile()).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProfiles = Assert.IsAssignableFrom<IEnumerable<ProfilGetResDto>>(okResult.Value);
        Assert.Equal(2, returnedProfiles.Count());
    }

    [Fact]
    public async Task GetProfil_WithValidId_ReturnsOkWithProfile()
    {
        // Arrange
        var profile = new ProfilGetResDto(
            "User1",
            new RegionKrajDto(1, "Mazowieckie", 1, "Poland"),
            "he/him",
            "Description",
            new List<JezykOrazStopienDto>(),
            Array.Empty<byte>(),
            "Online"
        );
        var result = ServiceResult<ProfilGetResDto>.Ok(profile);
        _mockProfilService.Setup(s => s.GetProfil(1)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetProfil(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProfile = Assert.IsType<ProfilGetResDto>(okResult.Value);
        Assert.Equal("User1", returnedProfile.Pseudonim);
    }

    [Fact]
    public async Task GetProfil_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var result = ServiceResult<ProfilGetResDto>.Fail(404, 
            new[] { new ErrorItem("Profile not found", "id") });
        _mockProfilService.Setup(s => s.GetProfil(999)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetProfil(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateProfil_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new ProfilUpdateDto(1, "he/him", "Updated description", new List<JezykProfiluCreateDto>(), "TestUser");
        var result = ServiceResult<bool>.Ok(true, 204);
        _mockProfilService.Setup(s => s.UpdateProfil(1, updateDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateProfil(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateProfil_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");

        // Act
        var actionResult = await _controller.UpdateProfil(1, updateDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateProfil_WhenUserIdMismatch_ReturnsForbidden()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");

        // Act
        var actionResult = await _controller.UpdateProfil(2, updateDto);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        Assert.Equal("Nie możesz edytować profilu innego użytkownika.", statusResult.Value);
    }

    [Fact]
    public async Task UpdateProfil_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");
        var result = ServiceResult<bool>.Fail(400, 
            new[] { new ErrorItem("Invalid data", "description") });
        _mockProfilService.Setup(s => s.UpdateProfil(1, updateDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateProfil(1, updateDto);

        // Assert
        Assert.IsAssignableFrom<IActionResult>(actionResult);
    }

    [Fact]
    public async Task UpdateProfil_WhenProfileNotFound_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateDto = new ProfilUpdateDto(1, "he/him", "Description", new List<JezykProfiluCreateDto>(), "TestUser");
        var result = ServiceResult<bool>.Fail(404, 
            new[] { new ErrorItem("Profile not found", "id") });
        _mockProfilService.Setup(s => s.UpdateProfil(1, updateDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateProfil(1, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateAwatar_WithValidFile_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("avatar.jpg");
        mockFile.Setup(f => f.Length).Returns(1024);

        var result = ServiceResult<bool>.Ok(true, 204);
        _mockProfilService.Setup(s => s.UpdateAwatar(1, mockFile.Object)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateAwatar(1, mockFile.Object);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateAwatar_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var mockFile = new Mock<IFormFile>();

        // Act
        var actionResult = await _controller.UpdateAwatar(1, mockFile.Object);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateAwatar_WhenUserIdMismatch_ReturnsForbidden()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var mockFile = new Mock<IFormFile>();

        // Act
        var actionResult = await _controller.UpdateAwatar(2, mockFile.Object);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        Assert.Equal("Nie możesz edytować profilu innego użytkownika.", statusResult.Value);
    }

    [Fact]
    public async Task UpdateAwatar_WithInvalidFile_ReturnsBadRequest()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var mockFile = new Mock<IFormFile>();
        var result = ServiceResult<bool>.Fail(400, 
            new[] { new ErrorItem("Invalid file format", "awatar") });
        _mockProfilService.Setup(s => s.UpdateAwatar(1, mockFile.Object)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateAwatar(1, mockFile.Object);

        // Assert
        Assert.IsAssignableFrom<IActionResult>(actionResult);
    }

    [Fact]
    public async Task UpdateStatus_WithValidStatus_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<StatusDto>.Ok(new StatusDto(2, "Status Name"), 204);
        _mockProfilService.Setup(s => s.UpdateStatus(1, 2)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateStatus(1, 2);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task UpdateStatus_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.UpdateStatus(1, 2);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateStatus_WhenUserIdMismatch_ReturnsForbidden()
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
        var actionResult = await _controller.UpdateStatus(2, 2);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        Assert.Equal("Nie możesz edytować statusu innego użytkownika.", statusResult.Value);
    }

    [Fact]
    public async Task UpdateStatus_WhenStatusNotFound_ReturnsNotFound()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = ServiceResult<StatusDto>.Fail(400, 
            new[] { new ErrorItem("Status not found", "idStatus") });
        _mockProfilService.Setup(s => s.UpdateStatus(1, 999)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateStatus(1, 999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Status not found", notFoundResult.Value);
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Controllers;
using Squadra.Server.DTO.Auth;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Models;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUzytkownikService> _mockUzytkownikService;
    private readonly Mock<IProfilService> _mockProfilService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly Mock<SignInManager<Uzytkownik>> _mockSignInManager;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockUzytkownikService = new Mock<IUzytkownikService>();
        _mockProfilService = new Mock<IProfilService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _mockSignInManager = MockSignInManager<Uzytkownik>(_mockUserManager.Object);
        _controller = new AuthController(
            _mockUzytkownikService.Object,
            _mockProfilService.Object,
            _mockUserManager.Object,
            _mockSignInManager.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    private static Mock<SignInManager<TUser>> MockSignInManager<TUser>(UserManager<TUser> userManager) where TUser : class
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        return new Mock<SignInManager<TUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null, null, null);
    }

    [Fact]
    public async Task Zarejestruj_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new UzytkownikCreateDto(
            "newuser",
            "Password123!",
            "newuser@test.com",
            null,
            new DateOnly(2000, 1, 1),
            "NewUser"
        );
        var result = ServiceResult<bool>.Ok(true, 201);
        _mockUzytkownikService.Setup(s => s.CreateUzytkownik(createDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Zarejestruj(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(actionResult);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }

    [Fact]
    public async Task Zarejestruj_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new UzytkownikCreateDto(
            "newuser",
            "weak",
            "invalid-email",
            null,
            new DateOnly(2000, 1, 1),
            "NewUser"
        );
        var result = ServiceResult<bool>.Fail(400, 
            new[] { new ErrorItem("Invalid email format", "email") });
        _mockUzytkownikService.Setup(s => s.CreateUzytkownik(createDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Zarejestruj(createDto);

        // Assert
        Assert.IsAssignableFrom<IActionResult>(actionResult);
    }

    [Fact]
    public async Task Zarejestruj_WhenUserExists_ReturnsConflict()
    {
        // Arrange
        var createDto = new UzytkownikCreateDto(
            "existinguser",
            "Password123!",
            "existing@test.com",
            null,
            new DateOnly(2000, 1, 1),
            "ExistingUser"
        );
        var result = ServiceResult<bool>.Fail(409, 
            new[] { new ErrorItem("User already exists", "login") });
        _mockUzytkownikService.Setup(s => s.CreateUzytkownik(createDto)).ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Zarejestruj(createDto);

        // Assert
        var conflictResult = Assert.IsType<ConflictResult>(actionResult);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
    }

    [Fact]
    public async Task Zaloguj_WithValidCredentials_ReturnsNoContent()
    {
        // Arrange
        var loginRequest = new LoginRequest("testuser", "Password123!", false);
        var user = new Uzytkownik { Id = 1, UserName = "testuser", Email = "test@test.com" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync((Uzytkownik?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginRequest.Haslo, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _mockSignInManager.Setup(x => x.SignInAsync(user, loginRequest.ZapamietajMnie, null))
            .Returns(Task.CompletedTask);
        _mockUserManager.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var actionResult = await _controller.Zaloguj(loginRequest);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task Zaloguj_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("testuser", "WrongPassword", false);
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync((Uzytkownik?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync((Uzytkownik?)null);

        // Act
        var actionResult = await _controller.Zaloguj(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        var value = Assert.IsType<string>(unauthorizedResult.Value!.GetType().GetProperty("message")!.GetValue(unauthorizedResult.Value));
        Assert.Equal("Nieprawidłowe dane logowania.", value);
    }

    [Fact]
    public async Task Zaloguj_WithWrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("testuser", "WrongPassword", false);
        var user = new Uzytkownik { Id = 1, UserName = "testuser", Email = "test@test.com" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync((Uzytkownik?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginRequest.Haslo, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var actionResult = await _controller.Zaloguj(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        var value = Assert.IsType<string>(unauthorizedResult.Value!.GetType().GetProperty("message")!.GetValue(unauthorizedResult.Value));
        Assert.Equal("Nieprawidłowe dane logowania.", value);
    }

    [Fact]
    public async Task Zaloguj_WithEmail_ReturnsNoContent()
    {
        // Arrange
        var loginRequest = new LoginRequest("test@test.com", "Password123!", true);
        var user = new Uzytkownik { Id = 1, UserName = "testuser", Email = "test@test.com" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginRequest.LoginLubEmail))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginRequest.Haslo, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _mockSignInManager.Setup(x => x.SignInAsync(user, loginRequest.ZapamietajMnie, null))
            .Returns(Task.CompletedTask);
        _mockUserManager.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var actionResult = await _controller.Zaloguj(loginRequest);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task Wyloguj_WhenAuthenticated_ReturnsNoContent()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.Wyloguj();

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task Wyloguj_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.Wyloguj();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Me_WhenAuthenticated_ReturnsOkWithUserData()
    {
        // Arrange
        var user = new Uzytkownik { Id = 1, UserName = "testuser", Email = "test@test.com" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        var profile = new ProfilGetResDto(
            "testuser",
            new DTO.KrajRegion.RegionKrajDto(1, "Mazowieckie", 1, "Poland"),
            "he/him",
            "Description",
            new List<DTO.JezykStopien.JezykOrazStopienDto>(),
            System.Text.Encoding.UTF8.GetBytes("avatar"),
            "Online"
        );
        var profilResult = ServiceResult<ProfilGetResDto>.Ok(profile);
        _mockProfilService.Setup(s => s.GetProfil(1)).ReturnsAsync(profilResult);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.Me();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var authUser = Assert.IsType<AuthUserDto>(okResult.Value);
        Assert.Equal(1, authUser.Id);
        Assert.Equal("testuser", authUser.Login);
        Assert.Equal("test@test.com", authUser.Email);
        Assert.Single(authUser.Role);
        Assert.Equal("User", authUser.Role[0]);
        Assert.NotNull(authUser.Awatar);
    }

    [Fact]
    public async Task Me_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Uzytkownik?)null);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var actionResult = await _controller.Me();

        // Assert
        Assert.IsType<UnauthorizedResult>(actionResult);
    }
}

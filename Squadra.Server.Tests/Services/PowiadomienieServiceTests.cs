using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Uzytkownicy.Services;
using Squadra.Server.Modules.Znajomosci.Models;
using Squadra.Server.Modules.Znajomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class PowiadomienieServiceTests
{
    private readonly Mock<IPowiadomienieRepository> _mockPowiadomienieRepository;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly Mock<IUzytkownikService> _mockUzytkownikService;
    private readonly Mock<IZnajomiService> _mockZnajomiService;
    private readonly Mock<IZnajomiRepository> _mockZnajomiRepository;
    private readonly Mock<IProfilService> _mockProfilService;
    private readonly PowiadomienieService _service;

    public PowiadomienieServiceTests()
    {
        _mockPowiadomienieRepository = new Mock<IPowiadomienieRepository>();
        _mockUserManager = MockUserManager();
        _mockUzytkownikService = new Mock<IUzytkownikService>();
        _mockZnajomiService = new Mock<IZnajomiService>();
        _mockZnajomiRepository = new Mock<IZnajomiRepository>();
        _mockProfilService = new Mock<IProfilService>();
        
        _service = new PowiadomienieService(
            _mockPowiadomienieRepository.Object,
            _mockUserManager.Object,
            _mockUzytkownikService.Object,
            _mockZnajomiService.Object,
            _mockZnajomiRepository.Object,
            _mockProfilService.Object
        );
    }

    private static Mock<UserManager<Uzytkownik>> MockUserManager()
    {
        var store = new Mock<IUserStore<Uzytkownik>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        var passwordHasher = new Mock<IPasswordHasher<Uzytkownik>>();
        var userValidators = Array.Empty<IUserValidator<Uzytkownik>>();
        var passwordValidators = Array.Empty<IPasswordValidator<Uzytkownik>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new IdentityErrorDescriber();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<Uzytkownik>>>();

        return new Mock<UserManager<Uzytkownik>>(
            store.Object,
            options.Object,
            passwordHasher.Object,
            userValidators,
            passwordValidators,
            keyNormalizer.Object,
            errors,
            services.Object,
            logger.Object);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    private static PowiadomienieDto PowiadomienieDto(
        int id,
        int idTypuPowiadomienia,
        int uzytkownikId,
        int? idPowiazanegoObiektu,
        string? nazwaPowiazanegoObiektu,
        string? tresc = null,
        string? dataWyslania = null)
        => new(id, idTypuPowiadomienia, uzytkownikId, idPowiazanegoObiektu, nazwaPowiazanegoObiektu, tresc, dataWyslania ?? DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

    private static PowiadomienieCreateDto PowiadomienieCreateDto(
        int idTypuPowiadomienia,
        int idUzytkownika,
        int? idPowiazanegoObiektu,
        string? nazwaPowiazanegoObiektu,
        string? tresc = null)
        => new(idTypuPowiadomienia, idUzytkownika, idPowiazanegoObiektu, nazwaPowiazanegoObiektu, tresc);

    private static List<Znajomi> StworzPelnaListeZnajomych(int ownerId)
    {
        return Enumerable.Range(1, ZnajomiService.MaxLiczbaZnajomych)
            .Select(i => new Znajomi
            {
                IdUzytkownika1 = ownerId,
                IdUzytkownika2 = 1000 + i,
                DataNawiazaniaZnajomosci = DateOnly.FromDateTime(DateTime.Now)
            })
            .ToList();
    }

    #region GetPowiadomienie Tests

    [Fact]
    public async Task GetPowiadomienie_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var notificationId = 1;
        var user = CreateClaimsPrincipal(1);
        var notification = PowiadomienieDto(notificationId, 2, 1, 2, "TestUser", "Test");
        
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomienie(notificationId))
            .ReturnsAsync(notification);
        _mockUserManager.Setup(um => um.GetUserAsync(user))
            .ReturnsAsync((Uzytkownik?)null);

        // Act
        var result = await _service.GetPowiadomienie(notificationId, user);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(401, result.StatusCode);
        Assert.Contains("zalogowany", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetPowiadomienie_WhenUserTriesToAccessOthersNotification_ReturnsForbidden()
    {
        // Arrange
        var notificationId = 1;
        var userId = 1;
        var otherUserId = 2;
        var user = CreateClaimsPrincipal(userId);
        var notification = PowiadomienieDto(notificationId, 2, otherUserId, 3, "TestUser", "Test");
        var uzytkownik = new Uzytkownik { Id = userId };
        
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomienie(notificationId))
            .ReturnsAsync(notification);
        _mockUserManager.Setup(um => um.GetUserAsync(user))
            .ReturnsAsync(uzytkownik);

        // Act
        var result = await _service.GetPowiadomienie(notificationId, user);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("innego użytkownika", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetPowiadomienie_WhenUserOwnsNotification_ReturnsOk()
    {
        // Arrange
        var notificationId = 1;
        var userId = 1;
        var user = CreateClaimsPrincipal(userId);
        var notification = PowiadomienieDto(notificationId, 2, userId, 2, "TestUser", "Test");
        var uzytkownik = new Uzytkownik { Id = userId };
        
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomienie(notificationId))
            .ReturnsAsync(notification);
        _mockUserManager.Setup(um => um.GetUserAsync(user))
            .ReturnsAsync(uzytkownik);

        // Act
        var result = await _service.GetPowiadomienie(notificationId, user);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(notificationId, result.Value.Id);
    }

    #endregion

    #region GetPowiadomieniaUzytkownika Tests

    [Fact]
    public async Task GetPowiadomieniaUzytkownika_ReturnsOkWithNotificationsList()
    {
        // Arrange
        var userId = 1;
        var notifications = new List<PowiadomienieDto>
        {
            new PowiadomienieDto(1, 2, userId, 2, "User1", "Test1", DateTime.Now.ToString("dd.MM.yyyy HH:mm")),
            new PowiadomienieDto(2, 3, userId, 2, "User2", "Test2", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
        };
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(userId))
            .ReturnsAsync(notifications);

        // Act
        var result = await _service.GetPowiadomieniaUzytkownika(userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    #endregion

    #region CreatePowiadomienie Tests

    [Fact]
    public async Task CreatePowiadomienie_WithInvalidNotificationType_ReturnsNotFound()
    {
        // Arrange
        var dto = PowiadomienieCreateDto(0, 1, 2, "TestUser", "Test");

        // Act
        var result = await _service.CreatePowiadomienie(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Typ powiadomienia", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreatePowiadomienie_WithInvalidRelatedObjectId_ReturnsNotFound()
    {
        // Arrange
        var dto = PowiadomienieCreateDto(2, 1, 0, "TestUser", "Test");

        // Act
        var result = await _service.CreatePowiadomienie(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Obiekt", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreatePowiadomienie_FriendRequestType_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var dto = PowiadomienieCreateDto(2, 1, 2, "TestUser", "Test");
        var userResult = ServiceResult<UzytkownikResDto>.NotFound(new ErrorItem("User not found"));
        
        _mockUzytkownikService.Setup(s => s.GetUzytkownik(2))
            .ReturnsAsync(userResult);

        // Act
        var result = await _service.CreatePowiadomienie(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CreatePowiadomienie_FriendRequestType_WhenUserExists_ReturnsNoContent()
    {
        // Arrange
        var dto = PowiadomienieCreateDto(2, 1, 2, "TestUser", "Test");
        var userDto = new UzytkownikResDto(2, "testuser", "test@test.com", "123456789", new DateOnly(1990, 1, 1), new[] { "User" });
        var userResult = ServiceResult<UzytkownikResDto>.Ok(userDto);
        
        _mockUzytkownikService.Setup(s => s.GetUzytkownik(2))
            .ReturnsAsync(userResult);
        _mockPowiadomienieRepository.Setup(r => r.CreatePowiadomienie(dto))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreatePowiadomienie(dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        _mockPowiadomienieRepository.Verify(r => r.CreatePowiadomienie(dto), Times.Once);
    }

    [Fact]
    public async Task CreatePowiadomienie_SystemNotificationType_ReturnsNoContent()
    {
        // Arrange
        var dto = PowiadomienieCreateDto(1, 1, 5, "System", "System notification");
        
        _mockPowiadomienieRepository.Setup(r => r.CreatePowiadomienie(dto))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreatePowiadomienie(dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
    }

    #endregion

    #region WyslijZaproszenieDoZnajomych Tests

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WithEmptyLogin_ReturnsNotFound()
    {
        // Arrange
        var inviterId = 1;
        var inviteeLogin = "";

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("login", result.Errors[0].Message.ToLower());
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenInviteeNotFound_ReturnsNotFound()
    {
        // Arrange
        var inviterId = 1;
        var inviteeLogin = "nonexistent";
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync((Uzytkownik?)null);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenInvitingSelf_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var login = "testuser";
        var uzytkownik = new Uzytkownik { Id = userId, UserName = login };
        
        _mockUserManager.Setup(um => um.FindByNameAsync(login))
            .ReturnsAsync(uzytkownik);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(userId, login);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("samego siebie", result.Errors[0].Message);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenInvitationAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var inviterId = 1;
        var inviteeId = 2;
        var inviteeLogin = "invitee";
        var invitee = new Uzytkownik { Id = inviteeId, UserName = inviteeLogin };
        var existingNotifications = new List<PowiadomienieDto>
        {
            new PowiadomienieDto(1, 2, inviteeId, inviterId, "Inviter", null, DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
        };
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(existingNotifications);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains("już wysłane zaproszenie", result.Errors[0].Message);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenAlreadyFriends_ReturnsConflict()
    {
        // Arrange
        var inviterId = 1;
        var inviteeId = 2;
        var inviteeLogin = "invitee";
        var invitee = new Uzytkownik { Id = inviteeId, UserName = inviteeLogin };
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(new List<PowiadomienieDto>());
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(inviteeId, inviterId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains("już Twoim znajomym", result.Errors[0].Message);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenInviterHasMaxFriends_ReturnsConflict()
    {
        // Arrange
        var inviterId = 1;
        var inviteeId = 2;
        var inviteeLogin = "invitee";
        var invitee = new Uzytkownik { Id = inviteeId, UserName = inviteeLogin };
        var friendsResult = ServiceResult<ICollection<Znajomi>>.Ok(StworzPelnaListeZnajomych(inviterId));
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(new List<PowiadomienieDto>());
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(inviteeId, inviterId))
            .ReturnsAsync(false);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(inviterId))
            .ReturnsAsync(friendsResult);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains("maksymalną liczbę znajomych", result.Errors[0].Message);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenInviteeHasMaxFriends_ReturnsConflict()
    {
        // Arrange
        var inviterId = 1;
        var inviteeId = 2;
        var inviteeLogin = "invitee";
        var invitee = new Uzytkownik { Id = inviteeId, UserName = inviteeLogin };
        var inviterFriendsResult = ServiceResult<ICollection<Znajomi>>.Ok(new List<Znajomi>());
        var inviteeFriendsResult = ServiceResult<ICollection<Znajomi>>.Ok(StworzPelnaListeZnajomych(inviteeId));
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(new List<PowiadomienieDto>());
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(inviteeId, inviterId))
            .ReturnsAsync(false);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(inviterId))
            .ReturnsAsync(inviterFriendsResult);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(inviteeId))
            .ReturnsAsync(inviteeFriendsResult);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains("maksymalną liczbę znajomych", result.Errors[0].Message);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var inviterId = 1;
        var inviteeId = 2;
        var inviteeLogin = "invitee";
        var invitee = new Uzytkownik { Id = inviteeId, UserName = inviteeLogin };
        var inviterFriendsResult = ServiceResult<ICollection<Znajomi>>.Ok(new List<Znajomi>());
        var inviteeFriendsResult = ServiceResult<ICollection<Znajomi>>.Ok(new List<Znajomi>());
        var profileResult = ServiceResult<ProfilGetResDto>.Ok(
            new ProfilGetResDto("Inviter", null, null, null, new List<JezykOrazStopienDto>(), null, "Active"));
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(new List<PowiadomienieDto>());
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(inviteeId, inviterId))
            .ReturnsAsync(false);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(inviterId))
            .ReturnsAsync(inviterFriendsResult);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(inviteeId))
            .ReturnsAsync(inviteeFriendsResult);
        _mockProfilService.Setup(s => s.GetProfil(inviterId))
            .ReturnsAsync(profileResult);
        _mockPowiadomienieRepository.Setup(r => r.CreatePowiadomienie(It.IsAny<PowiadomienieCreateDto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        _mockPowiadomienieRepository.Verify(r => r.CreatePowiadomienie(It.IsAny<PowiadomienieCreateDto>()), Times.Once);
    }

    [Fact]
    public async Task WyslijZaproszenieDoZnajomych_WhenRepositoryThrowsException_ReturnsNotFound()
    {
        // Arrange
        var inviterId = 1;
        var inviteeLogin = "invitee";
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ThrowsAsync(new NieZnalezionoWBazieException("Database error"));

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomychPoLoginie(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}

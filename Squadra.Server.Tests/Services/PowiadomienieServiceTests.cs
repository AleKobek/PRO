using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
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
        return new Mock<UserManager<Uzytkownik>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    #region GetPowiadomienie Tests

    [Fact]
    public async Task GetPowiadomienie_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var notificationId = 1;
        var user = CreateClaimsPrincipal(1);
        var notification = new PowiadomienieDto(notificationId, 2, 1, 2, "TestUser", "Test", DateTime.Now);
        
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
        var notification = new PowiadomienieDto(notificationId, 2, otherUserId, 3, "TestUser", "Test", DateTime.Now);
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
        var notification = new PowiadomienieDto(notificationId, 2, userId, 2, "TestUser", "Test", DateTime.Now);
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
            new PowiadomienieDto(1, 2, userId, 2, "User1", "Test1", DateTime.Now),
            new PowiadomienieDto(2, 3, userId, 2, "User2", "Test2", DateTime.Now)
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
        var dto = new PowiadomienieCreateDto(0, 1, 2, "TestUser", "Test");

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
        var dto = new PowiadomienieCreateDto(2, 1, 0, "TestUser", "Test");

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
        var dto = new PowiadomienieCreateDto(2, 1, 2, "TestUser", "Test");
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
        var dto = new PowiadomienieCreateDto(2, 1, 2, "TestUser", "Test");
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
        var dto = new PowiadomienieCreateDto(1, 1, 5, "System", "System notification");
        
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
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var result = await _service.WyslijZaproszenieDoZnajomych(userId, login);

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
            new PowiadomienieDto(1, 2, inviteeId, inviterId, "Inviter", null, DateTime.Now)
        };
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(existingNotifications);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var maxFriends = new List<ProfilGetResDto>();
        for (int i = 0; i < ZnajomiService.MaxLiczbaZnajomych; i++)
        {
            maxFriends.Add(new ProfilGetResDto($"Friend{i}", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active"));
        }
        var friendsResult = ServiceResult<ICollection<ProfilGetResDto>>.Ok(maxFriends);
        
        _mockUserManager.Setup(um => um.FindByNameAsync(inviteeLogin))
            .ReturnsAsync(invitee);
        _mockPowiadomienieRepository.Setup(r => r.GetPowiadomieniaUzytkownika(inviteeId))
            .ReturnsAsync(new List<PowiadomienieDto>());
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(inviteeId, inviterId))
            .ReturnsAsync(false);
        _mockZnajomiService.Setup(s => s.GetZnajomiUzytkownika(inviterId))
            .ReturnsAsync(friendsResult);

        // Act
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var maxFriends = new List<ProfilGetResDto>();
        for (int i = 0; i < ZnajomiService.MaxLiczbaZnajomych; i++)
        {
            maxFriends.Add(new ProfilGetResDto($"Friend{i}", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active"));
        }
        var inviterFriendsResult = ServiceResult<ICollection<ProfilGetResDto>>.Ok(new List<ProfilGetResDto>());
        var inviteeFriendsResult = ServiceResult<ICollection<ProfilGetResDto>>.Ok(maxFriends);
        
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
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var inviterFriendsResult = ServiceResult<ICollection<ProfilGetResDto>>.Ok(new List<ProfilGetResDto>());
        var inviteeFriendsResult = ServiceResult<ICollection<ProfilGetResDto>>.Ok(new List<ProfilGetResDto>());
        var profileResult = ServiceResult<ProfilGetResDto>.Ok(
            new ProfilGetResDto("Inviter", null, null, null, new List<DTO.JezykStopien.JezykOrazStopienDto>(), null, "Active"));
        
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
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

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
        var result = await _service.WyslijZaproszenieDoZnajomych(inviterId, inviteeLogin);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}

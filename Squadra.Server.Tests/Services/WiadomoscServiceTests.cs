using Moq;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Wiadomosci.DTO;
using Squadra.Server.Modules.Wiadomosci.Repositories;
using Squadra.Server.Modules.Wiadomosci.Services;
using Squadra.Server.Modules.Znajomosci.Repositories;

namespace Squadra.Server.Tests.Services;

public class WiadomoscServiceTests
{
    private readonly Mock<IWiadomoscRepository> _mockWiadomoscRepository;
    private readonly Mock<IZnajomiRepository> _mockZnajomiRepository;
    private readonly Mock<IDruzynyService> _mockDruzynyService;
    private readonly Mock<IProfilService> _mockProfilService;
    private readonly WiadomoscService _service;

    public WiadomoscServiceTests()
    {
        _mockWiadomoscRepository = new Mock<IWiadomoscRepository>();
        _mockZnajomiRepository = new Mock<IZnajomiRepository>();
        _mockDruzynyService = new Mock<IDruzynyService>();
        _mockProfilService = new Mock<IProfilService>();
        _service = new WiadomoscService(
            _mockWiadomoscRepository.Object, 
            _mockZnajomiRepository.Object,
            _mockDruzynyService.Object,
            _mockProfilService.Object
        );
    }

    #region GetWiadomosc Tests

    [Fact]
    public async Task GetWiadomosc_WithIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var messageId = 0;
        var userId = 1;

        // Act
        var result = await _service.GetWiadomosc(messageId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetWiadomosc_WhenUserIsRecipient_ReturnsOk()
    {
        // Arrange
        var messageId = 1;
        var userId = 2;
        var message = new WiadomoscDto(1, userId, "01.01.2026 12:30", "Test message", 1);
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosc(messageId))
            .ReturnsAsync(message);

        // Act
        var result = await _service.GetWiadomosc(messageId, userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetWiadomosc_WhenUserIsSender_ReturnsOk()
    {
        // Arrange
        var messageId = 1;
        var userId = 1;
        var message = new WiadomoscDto(userId, 2, "01.01.2026 12:30", "Test message", 1);
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosc(messageId))
            .ReturnsAsync(message);

        // Act
        var result = await _service.GetWiadomosc(messageId, userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetWiadomosc_WhenUserIsNotParticipant_ReturnsForbidden()
    {
        // Arrange
        var messageId = 1;
        var userId = 3; // Not sender or recipient
        var message = new WiadomoscDto(1, 2, "01.01.2026 12:30", "Test message", 1);
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosc(messageId))
            .ReturnsAsync(message);

        // Act
        var result = await _service.GetWiadomosc(messageId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Brak dostępu", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetWiadomosc_WhenMessageNotFound_ReturnsNotFound()
    {
        // Arrange
        var messageId = 999;
        var userId = 1;
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosc(messageId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Wiadomość nie istnieje"));

        // Act
        var result = await _service.GetWiadomosc(messageId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region GetWiadomosci Tests

    [Fact]
    public async Task GetWiadomosci_WithValidIds_ReturnsOk()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        var messages = new List<WiadomoscDto>
        {
            new WiadomoscDto(userId1, userId2, "01.01.2026 12:30", "Message 1", 1),
            new WiadomoscDto(userId2, userId1, "01.01.2026 12:31", "Message 2", 1)
        };
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosciPrywatne(userId1, userId2))
            .ReturnsAsync(messages);

        // Act
        var result = await _service.GetWiadomosciPrywatne(userId1, userId2);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetWiadomosci_WithFirstIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var userId1 = 0;
        var userId2 = 2;

        // Act
        var result = await _service.GetWiadomosciPrywatne(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetWiadomosci_WithSecondIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 0;

        // Act
        var result = await _service.GetWiadomosciPrywatne(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetWiadomosci_WithSameIds_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetWiadomosciPrywatne(userId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("tym samym użytkownikiem", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetWiadomosci_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 999;
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosciPrywatne(userId1, userId2))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.GetWiadomosciPrywatne(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region CreateWiadomosc Tests

    [Fact]
    public async Task CreateWiadomosc_WithValidData_ReturnsCreated()
    {
        // Arrange
        var senderId = 1;
        var recipientId = 2;
        var dto = new WiadomoscCreateDto("Test message content", 1);
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(senderId, recipientId))
            .ReturnsAsync(true);
        _mockWiadomoscRepository.Setup(r => r.CreateWiadomosc(recipientId, dto, senderId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateWiadomoscPrywatna(recipientId, "Test message content", senderId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(201, result.StatusCode);
        _mockWiadomoscRepository.Verify(r => r.CreateWiadomosc(recipientId, dto, senderId), Times.Once);
    }

    [Fact]
    public async Task CreateWiadomosc_WhenSenderAndRecipientAreSame_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var result = await _service.CreateWiadomoscPrywatna(userId, "Test message", userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("tym samym użytkownikiem", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateWiadomosc_WithEmptyContent_ReturnsBadRequest()
    {
        // Arrange
        var senderId = 1;

        // Act
        var result = await _service.CreateWiadomoscPrywatna(2,"", senderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("nie może być pusta", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateWiadomosc_WithNullContent_ReturnsBadRequest()
    {
        // Arrange
        var senderId = 1;

        // Act
        var result = await _service.CreateWiadomoscPrywatna(2, null!, senderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("nie może być pusta", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateWiadomosc_WithContentExceeding1000Characters_ReturnsBadRequest()
    {
        // Arrange
        var senderId = 1;
        var longContent = new string('a', 1001); // 1001 characters

        // Act
        var result = await _service.CreateWiadomoscPrywatna(2, longContent, senderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("1000 znaków", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateWiadomosc_WhenUsersAreNotFriends_ReturnsBadRequest()
    {
        // Arrange
        var senderId = 1;
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(senderId, 2))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateWiadomoscPrywatna(2, "Test message", senderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("znajomym", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateWiadomosc_WhenRecipientNotFound_ReturnsNotFound()
    {
        // Arrange
        var senderId = 1;
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(senderId, 999))
            .ThrowsAsync(new NieZnalezionoWBazieException("Odbiorca nie istnieje"));

        // Act
        var result = await _service.CreateWiadomoscPrywatna(999, "Test message", senderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region DeleteWiadomosciUzytkownikow Tests

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithValidIds_ReturnsOk()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        _mockWiadomoscRepository.Setup(r => r.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        _mockWiadomoscRepository.Verify(r => r.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2), Times.Once);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithFirstIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var userId1 = 0;
        var userId2 = 2;

        // Act
        var result = await _service.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithSecondIdLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = -1;

        // Act
        var result = await _service.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithSameIds_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.DeleteWiadomosciPrywatneUzytkownikow(userId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("tym samym użytkownikiem", result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WhenRepositoryThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 999;
        _mockWiadomoscRepository.Setup(r => r.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.DeleteWiadomosciPrywatneUzytkownikow(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}

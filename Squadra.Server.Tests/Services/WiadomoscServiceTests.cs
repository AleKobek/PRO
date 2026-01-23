using Moq;
using Squadra.Server.DTO.Wiadomosc;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class WiadomoscServiceTests
{
    private readonly Mock<IWiadomoscRepository> _mockWiadomoscRepository;
    private readonly Mock<IZnajomiRepository> _mockZnajomiRepository;
    private readonly WiadomoscService _service;

    public WiadomoscServiceTests()
    {
        _mockWiadomoscRepository = new Mock<IWiadomoscRepository>();
        _mockZnajomiRepository = new Mock<IZnajomiRepository>();
        _service = new WiadomoscService(_mockWiadomoscRepository.Object, _mockZnajomiRepository.Object);
    }

    #region GetWiadomosc Tests

    [Fact]
    public async Task GetWiadomosc_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var messageId = 0;
        var userId = 1;

        // Act
        var result = await _service.GetWiadomosc(messageId, userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetWiadomosc_WhenUserIsRecipient_ReturnsOk()
    {
        // Arrange
        var messageId = 1;
        var userId = 2;
        var message = new WiadomoscDto(1, userId, DateTime.Now, "Test message", 1);
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
        var message = new WiadomoscDto(userId, 2, DateTime.Now, "Test message", 1);
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
        var message = new WiadomoscDto(1, 2, DateTime.Now, "Test message", 1);
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
            new WiadomoscDto(userId1, userId2, DateTime.Now, "Message 1", 1),
            new WiadomoscDto(userId2, userId1, DateTime.Now, "Message 2", 1)
        };
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosci(userId1, userId2))
            .ReturnsAsync(messages);

        // Act
        var result = await _service.GetWiadomosci(userId1, userId2);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetWiadomosci_WithFirstIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 0;
        var userId2 = 2;

        // Act
        var result = await _service.GetWiadomosci(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetWiadomosci_WithSecondIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 0;

        // Act
        var result = await _service.GetWiadomosci(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetWiadomosci_WithSameIds_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetWiadomosci(userId, userId);

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
        _mockWiadomoscRepository.Setup(r => r.GetWiadomosci(userId1, userId2))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.GetWiadomosci(userId1, userId2);

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
        var dto = new WiadomoscCreateDto(2, "Test message content", 1);
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(senderId, dto.IdOdbiorcy))
            .ReturnsAsync(true);
        _mockWiadomoscRepository.Setup(r => r.CreateWiadomosc(dto, senderId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateWiadomosc(dto, senderId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(201, result.StatusCode);
        _mockWiadomoscRepository.Verify(r => r.CreateWiadomosc(dto, senderId), Times.Once);
    }

    [Fact]
    public async Task CreateWiadomosc_WhenSenderAndRecipientAreSame_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var dto = new WiadomoscCreateDto(userId, "Test message", 1);

        // Act
        var result = await _service.CreateWiadomosc(dto, userId);

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
        var dto = new WiadomoscCreateDto(2, "", 1);

        // Act
        var result = await _service.CreateWiadomosc(dto, senderId);

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
        var dto = new WiadomoscCreateDto(2, null!, 1);

        // Act
        var result = await _service.CreateWiadomosc(dto, senderId);

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
        var dto = new WiadomoscCreateDto(2, longContent, 1);

        // Act
        var result = await _service.CreateWiadomosc(dto, senderId);

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
        var dto = new WiadomoscCreateDto(2, "Test message", 1);
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(senderId, dto.IdOdbiorcy))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateWiadomosc(dto, senderId);

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
        var dto = new WiadomoscCreateDto(999, "Test message", 1);
        _mockZnajomiRepository.Setup(r => r.CzyJestZnajomosc(senderId, dto.IdOdbiorcy))
            .ThrowsAsync(new NieZnalezionoWBazieException("Odbiorca nie istnieje"));

        // Act
        var result = await _service.CreateWiadomosc(dto, senderId);

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
        _mockWiadomoscRepository.Setup(r => r.DeleteWiadomosciUzytkownikow(userId1, userId2))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteWiadomosciUzytkownikow(userId1, userId2);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        _mockWiadomoscRepository.Verify(r => r.DeleteWiadomosciUzytkownikow(userId1, userId2), Times.Once);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithFirstIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 0;
        var userId2 = 2;

        // Act
        var result = await _service.DeleteWiadomosciUzytkownikow(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithSecondIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = -1;

        // Act
        var result = await _service.DeleteWiadomosciUzytkownikow(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_WithSameIds_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.DeleteWiadomosciUzytkownikow(userId, userId);

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
        _mockWiadomoscRepository.Setup(r => r.DeleteWiadomosciUzytkownikow(userId1, userId2))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.DeleteWiadomosciUzytkownikow(userId1, userId2);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}

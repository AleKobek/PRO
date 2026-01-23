using Moq;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class UzytkownikServiceTests
{
    private readonly Mock<IUzytkownikRepository> _mockRepository;
    private readonly UzytkownikService _service;

    public UzytkownikServiceTests()
    {
        _mockRepository = new Mock<IUzytkownikRepository>();
        _service = new UzytkownikService(_mockRepository.Object);
    }

    #region GetUzytkownik Tests

    [Fact]
    public async Task GetUzytkownik_ById_WithValidId_ReturnsOk()
    {
        // Arrange
        var userId = 5;
        var expectedUser = new UzytkownikResDto(userId, "testuser", "test@example.com", "123456789", new DateOnly(1990, 1, 1), new[] { "User" });
        _mockRepository.Setup(r => r.GetUzytkownik(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.GetUzytkownik(userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.Id);
    }

    [Fact]
    public async Task GetUzytkownik_ById_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId = 0;

        // Act
        var result = await _service.GetUzytkownik(userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("nie istnieje", result.Errors[0].Message);
    }

    [Fact]
    public async Task GetUzytkownik_ByLogin_WithValidLogin_ReturnsOk()
    {
        // Arrange
        var login = "testuser";
        var expectedUser = new UzytkownikResDto(1, login, "test@example.com", "123456789", new DateOnly(1990, 1, 1), new[] { "User" });
        _mockRepository.Setup(r => r.GetUzytkownik(login))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.GetUzytkownik(login);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(login, result.Value.Login);
    }

    [Fact]
    public async Task GetUzytkownik_ByLogin_WithEmptyLogin_ReturnsNotFound()
    {
        // Arrange
        var login = "";

        // Act
        var result = await _service.GetUzytkownik(login);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetUzytkownik_ByLogin_WithWhitespaceLogin_ReturnsNotFound()
    {
        // Arrange
        var login = "   ";

        // Act
        var result = await _service.GetUzytkownik(login);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region UpdateHaslo Tests

    [Fact]
    public async Task UpdateHaslo_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId = 0;
        var oldPassword = "OldPass123!";
        var newPassword = "NewPass123!";

        // Act
        var result = await _service.UpdateHaslo(userId, oldPassword, newPassword);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task UpdateHaslo_WithSamePasswords_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var password = "SamePass123!";

        // Act
        var result = await _service.UpdateHaslo(userId, password, password);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("takie same", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateHaslo_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "OldPass123!";
        var newPassword = "weak"; // Invalid password

        // Act
        var result = await _service.UpdateHaslo(userId, oldPassword, newPassword);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Niepoprawne hasło", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateHaslo_WithValidPasswords_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "OldPass123!";
        var newPassword = "NewPass123!";
        _mockRepository.Setup(r => r.UpdateHaslo(userId, oldPassword, newPassword))
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _service.UpdateHaslo(userId, oldPassword, newPassword);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task UpdateHaslo_WhenRepositoryReturnsErrors_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "WrongOldPass123!";
        var newPassword = "NewPass123!";
        var errors = new List<string> { "Niepoprawne stare hasło" };
        _mockRepository.Setup(r => r.UpdateHaslo(userId, oldPassword, newPassword))
            .ReturnsAsync(errors);

        // Act
        var result = await _service.UpdateHaslo(userId, oldPassword, newPassword);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task UpdateHaslo_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        var oldPassword = "OldPass123!";
        var newPassword = "NewPass123!";
        _mockRepository.Setup(r => r.UpdateHaslo(userId, oldPassword, newPassword))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.UpdateHaslo(userId, oldPassword, newPassword);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region DeleteUzytkownik Tests

    [Fact]
    public async Task DeleteUzytkownik_WithIdLessThanOne_ReturnsNotFound()
    {
        // Arrange
        var userId = 0;

        // Act
        var result = await _service.DeleteUzytkownik(userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task DeleteUzytkownik_WithValidId_ReturnsOk()
    {
        // Arrange
        var userId = 5;
        _mockRepository.Setup(r => r.DeleteUzytkownik(userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteUzytkownik(userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        _mockRepository.Verify(r => r.DeleteUzytkownik(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUzytkownik_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        _mockRepository.Setup(r => r.DeleteUzytkownik(userId))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.DeleteUzytkownik(userId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region CreateUzytkownik Tests

    [Fact]
    public async Task CreateUzytkownik_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "invalid-email", // Invalid email
            "123456789",
            new DateOnly(1990, 1, 1),
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("email"));
    }

    [Fact]
    public async Task CreateUzytkownik_WithInvalidPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "test@example.com",
            "12345", // Invalid phone
            new DateOnly(1990, 1, 1),
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("telefon"));
    }

    [Fact]
    public async Task CreateUzytkownik_WithAgeLessThan18_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "test@example.com",
            "123456789",
            DateOnly.FromDateTime(DateTime.Now.AddYears(-10)), // Age < 18
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("data urodzenia"));
    }

    [Fact]
    public async Task CreateUzytkownik_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "weak", // Invalid password
            "test@example.com",
            "123456789",
            new DateOnly(1990, 1, 1),
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("hasło"));
    }

    [Fact]
    public async Task CreateUzytkownik_WithExistingLogin_ReturnsConflict()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "existinguser",
            "ValidPass123!",
            "test@example.com",
            "123456789",
            new DateOnly(1990, 1, 1),
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(true); // Login exists
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Code == "LoginIstnieje");
    }

    [Fact]
    public async Task CreateUzytkownik_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "existing@example.com",
            "123456789",
            new DateOnly(1990, 1, 1),
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(true); // Email exists

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Code == "EmailIstnieje");
    }

    [Fact]
    public async Task CreateUzytkownik_WithEmptyPseudonim_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "test@example.com",
            "123456789",
            new DateOnly(1990, 1, 1),
            "" // Empty pseudonim
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("pseudonim"));
    }

    [Fact]
    public async Task CreateUzytkownik_WithTooLongPseudonim_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "test@example.com",
            "123456789",
            new DateOnly(1990, 1, 1),
            "ThisPseudonymIsWayTooLongForTheSystem" // > 20 characters
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("pseudonim"));
    }

    [Fact]
    public async Task CreateUzytkownik_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new UzytkownikCreateDto(
            "testuser",
            "ValidPass123!",
            "test@example.com",
            "123456789",
            new DateOnly(1990, 1, 1),
            "TestPseudo"
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(null, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(null, dto.Email))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CreateUzytkownik(dto))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateUzytkownik(dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(201, result.StatusCode);
        _mockRepository.Verify(r => r.CreateUzytkownik(dto), Times.Once);
    }

    #endregion

    #region UpdateUzytkownik Tests

    [Fact]
    public async Task UpdateUzytkownik_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        var dto = new UzytkownikUpdateDto(
            "updateduser",
            "updated@example.com",
            "987654321",
            new DateOnly(1990, 1, 1)
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(userId, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(userId, dto.Email))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.UpdateUzytkownik(userId, dto))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateUzytkownik(userId, dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        _mockRepository.Verify(r => r.UpdateUzytkownik(userId, dto), Times.Once);
    }

    [Fact]
    public async Task UpdateUzytkownik_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var dto = new UzytkownikUpdateDto(
            "updateduser",
            "invalid-email",
            "987654321",
            new DateOnly(1990, 1, 1)
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(userId, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(userId, dto.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _service.UpdateUzytkownik(userId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Message.Contains("email"));
    }

    [Fact]
    public async Task UpdateUzytkownik_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        var dto = new UzytkownikUpdateDto(
            "updateduser",
            "updated@example.com",
            "987654321",
            new DateOnly(1990, 1, 1)
        );
        _mockRepository.Setup(r => r.CzyLoginIstnieje(userId, dto.Login))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CzyEmailIstnieje(userId, dto.Email))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.UpdateUzytkownik(userId, dto))
            .ThrowsAsync(new NieZnalezionoWBazieException("Użytkownik nie istnieje"));

        // Act
        var result = await _service.UpdateUzytkownik(userId, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}

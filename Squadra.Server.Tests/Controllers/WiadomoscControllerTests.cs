using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Wiadomosci.Controllers;
using Squadra.Server.Modules.Wiadomosci.DTO;
using Squadra.Server.Modules.Wiadomosci.Services;

namespace Squadra.Server.Tests.Controllers;

public class WiadomoscControllerTests
{
    private readonly Mock<IWiadomoscService> _mockWiadomoscService;
    private readonly Mock<IStatystykiCzatuService> _mockStatystykiCzatuService;
    private readonly Mock<UserManager<Uzytkownik>> _mockUserManager;
    private readonly WiadomoscController _controller;

    public WiadomoscControllerTests()
    {
        _mockWiadomoscService = new Mock<IWiadomoscService>();
        _mockStatystykiCzatuService = new Mock<IStatystykiCzatuService>();
        _mockUserManager = MockUserManager<Uzytkownik>();
        _controller = new WiadomoscController(
            _mockWiadomoscService.Object,
            _mockStatystykiCzatuService.Object,
            _mockUserManager.Object);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task GetWiadomosc_WithValidId_ReturnsOkWithMessage()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var messageDto = new WiadomoscDto(1, 2, "01.01.2026 12:30", "Hello!", 1);
        _mockWiadomoscService
            .Setup(s => s.GetWiadomosc(1, user.Id))
            .ReturnsAsync(ServiceResult<WiadomoscDto>.Ok(messageDto));

        var actionResult = await _controller.GetWiadomosc(1);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedMessage = Assert.IsType<WiadomoscDto>(okResult.Value);
        Assert.Equal("Hello!", returnedMessage.Tresc);
    }

    [Fact]
    public async Task GetWiadomosc_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var actionResult = await _controller.GetWiadomosc(1);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetWiadomosc_WithInvalidId_ReturnsNotFound()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        _mockWiadomoscService
            .Setup(s => s.GetWiadomosc(999, user.Id))
            .ReturnsAsync(ServiceResult<WiadomoscDto>.Fail(404, new[] { new ErrorItem("Message not found", "id") }));

        var actionResult = await _controller.GetWiadomosc(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Message not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetWiadomosc_WhenForbidden_ReturnsForbidden()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        _mockWiadomoscService
            .Setup(s => s.GetWiadomosc(1, user.Id))
            .ReturnsAsync(ServiceResult<WiadomoscDto>.Fail(403, new[] { new ErrorItem("Access forbidden", "id") }));

        var actionResult = await _controller.GetWiadomosc(1);

        var forbiddenResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, forbiddenResult.StatusCode);
        Assert.Equal("Access forbidden", forbiddenResult.Value);
    }

    [Fact]
    public async Task GetWiadomosci_WithValidRecipientId_ReturnsOkWithMessages()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        ICollection<WiadomoscDto> messages =
        [
            new WiadomoscDto(1, 2, "01.01.2026 12:30", "Hello!", 1),
            new WiadomoscDto(2, 1, "01.01.2026 12:31", "Hi there!", 1)
        ];

        _mockWiadomoscService
            .Setup(s => s.GetWiadomosci(user.Id, 2))
            .ReturnsAsync(ServiceResult<ICollection<WiadomoscDto>>.Ok(messages));

        var actionResult = await _controller.GetWiadomosci(2);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedMessages = Assert.IsAssignableFrom<IEnumerable<WiadomoscDto>>(okResult.Value);
        Assert.Equal(2, returnedMessages.Count());
    }

    [Fact]
    public async Task GetWiadomosci_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var actionResult = await _controller.GetWiadomosci(2);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task CzySaNoweWiadomosci_WhenAuthenticated_ReturnsOk()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _mockStatystykiCzatuService
            .Setup(s => s.CzySaNoweWiadomosciOdZnajomych(user.Id))
            .ReturnsAsync(ServiceResult<bool>.Ok(true));

        var actionResult = await _controller.CzySaNoweWiadomosci();

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task CzySaNoweWiadomosci_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var actionResult = await _controller.CzySaNoweWiadomosci();

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task CreateWiadomosc_WithValidData_ReturnsCreated()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var createDto = new WiadomoscCreateDto("Hello!", 1);
        _mockWiadomoscService
            .Setup(s => s.CreateWiadomosc(2, createDto, user.Id))
            .ReturnsAsync(ServiceResult<bool>.Ok(true, 201));

        var actionResult = await _controller.CreateWiadomosc(2, createDto);

        var createdResult = Assert.IsType<CreatedResult>(actionResult);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task CreateWiadomosc_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Uzytkownik?)null);

        var createDto = new WiadomoscCreateDto("Hello!", 1);
        var actionResult = await _controller.CreateWiadomosc(2, createDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal("Nie jesteś zalogowany.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task CreateWiadomosc_WithInvalidData_ReturnsBadRequest()
    {
        var user = new Uzytkownik { Id = 1, UserName = "testuser" };
        _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var createDto = new WiadomoscCreateDto("", 1);
        _mockWiadomoscService
            .Setup(s => s.CreateWiadomosc(2, createDto, user.Id))
            .ReturnsAsync(ServiceResult<bool>.Fail(400, new[] { new ErrorItem("Message content is required", "Tresc") }));

        var actionResult = await _controller.CreateWiadomosc(2, createDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Message content is required", badRequestResult.Value);
    }
}

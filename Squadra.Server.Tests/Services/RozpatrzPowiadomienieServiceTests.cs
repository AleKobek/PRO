using Moq;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Models;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Znajomosci.Services;
using Xunit;

namespace Squadra.Server.Tests.Services;

public class RozpatrzPowiadomienieServiceTests
{
    private readonly Mock<IPowiadomieniaRepository> _mockPowiadomieniaRepository;
    private readonly Mock<IZnajomosciService> _mockZnajomosciService;
    private readonly Mock<IProfileService> _mockProfileService;
    private readonly Mock<IDruzynyService> _mockDruzynyService;
    private readonly Mock<IStatystykiService> _mockStatystykiService;
    private readonly RozpatrzPowiadomienieService _service;

    public RozpatrzPowiadomienieServiceTests()
    {
        _mockPowiadomieniaRepository = new Mock<IPowiadomieniaRepository>();
        _mockZnajomosciService = new Mock<IZnajomosciService>();
        _mockProfileService = new Mock<IProfileService>();
        _mockDruzynyService = new Mock<IDruzynyService>();
        _mockStatystykiService = new Mock<IStatystykiService>();

        _service = new RozpatrzPowiadomienieService(
            _mockPowiadomieniaRepository.Object,
            _mockZnajomosciService.Object,
            _mockProfileService.Object,
            _mockDruzynyService.Object,
            _mockStatystykiService.Object
        );
    }

    private static Powiadomienie StworzPowiadomienieEncje(int id, int typ, int uzytkownikId, int? powiazanyId = null, int? drugiPowiazanyId = null)
        => new()
        {
            Id = id,
            TypPowiadomieniaId = typ,
            UzytkownikId = uzytkownikId,
            PowiazanyObiektId = powiazanyId,
            DrugiPowiazanyObiektId = drugiPowiazanyId,
            Tresc = null,
            DataWyslania = DateTime.Now
        };

    [Fact]
    public async Task RozpatrzPowiadomienie_WhenNotificationBelongsToDifferentUser_ReturnsForbidden()
    {
        // Arrange
        var notif = StworzPowiadomienieEncje(1, (int)TypPowiadomieniaEnum.ZaproszenieDoZnajomych, uzytkownikId: 2, powiazanyId: 3);
        _mockPowiadomieniaRepository.Setup(r => r.GetPowiadomienie(1)).ReturnsAsync(notif);

        // Act
        var res = await _service.RozpatrzPowiadomienie(1, true, idUzytkownika: 1);

        // Assert
        Assert.False(res.Succeeded);
        Assert.Equal(403, res.StatusCode);
        _mockPowiadomieniaRepository.Verify(r => r.DeletePowiadomienie(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RozpatrzPowiadomienie_ZaproszenieDoZnajomych_Accepted_CreatesZnajomoscAndNotification()
    {
        // Arrange
        var accepterId = 10;
        var senderId = 20;
        var notif = StworzPowiadomienieEncje(1, (int)TypPowiadomieniaEnum.ZaproszenieDoZnajomych, uzytkownikId: accepterId, powiazanyId: senderId);
        _mockPowiadomieniaRepository.Setup(r => r.GetPowiadomienie(1)).ReturnsAsync(notif);
        _mockZnajomosciService.Setup(z => z.CreateZnajomosc(accepterId, senderId)).ReturnsAsync(ServiceResult<bool>.Created(true));
        _mockProfileService.Setup(p => p.GetProfil(accepterId)).ReturnsAsync(ServiceResult<ProfilGetResDto>.Ok(new ProfilGetResDto("pseudo", null, null, null, Array.Empty<JezykOrazStopienDto>(), null, "")));
        _mockPowiadomieniaRepository.Setup(r => r.CreatePowiadomienie(It.IsAny<PowiadomienieCreateDto>())).ReturnsAsync(true);
        _mockPowiadomieniaRepository.Setup(r => r.DeletePowiadomienie(1)).ReturnsAsync(true);

        // Act
        var res = await _service.RozpatrzPowiadomienie(1, true, accepterId);

        // Assert
        Assert.True(res.Succeeded);
        Assert.Equal(204, res.StatusCode);
        _mockZnajomosciService.Verify(z => z.CreateZnajomosc(accepterId, senderId), Times.Once);
        _mockPowiadomieniaRepository.Verify(r => r.CreatePowiadomienie(It.Is<PowiadomienieCreateDto>(dto =>
            dto.IdTypuPowiadomienia == (int)TypPowiadomieniaEnum.PrzyjecieZaproszeniaDoZnajomych &&
            dto.IdUzytkownika == senderId &&
            dto.IdPowiazanegoObiektu == accepterId
        )), Times.Once);
        _mockPowiadomieniaRepository.Verify(r => r.DeletePowiadomienie(1), Times.Once);
    }

    [Fact]
    public async Task RozpatrzPowiadomienie_ZaproszenieDoZnajomych_NullAcceptance_ReturnsBadRequest()
    {
        // Arrange
        var userId = 5;
        var notif = StworzPowiadomienieEncje(2, (int)TypPowiadomieniaEnum.ZaproszenieDoZnajomych, uzytkownikId: userId, powiazanyId: 6);
        _mockPowiadomieniaRepository.Setup(r => r.GetPowiadomienie(2)).ReturnsAsync(notif);
        // profile must exist for the method to proceed to acceptance check
        _mockProfileService.Setup(p => p.GetProfil(It.IsAny<int>())).ReturnsAsync(ServiceResult<ProfilGetResDto>.Ok(new ProfilGetResDto("pseudo", null, null, null, Array.Empty<JezykOrazStopienDto>(), null, "")));

        // Act
        var res = await _service.RozpatrzPowiadomienie(2, null, userId);

        // Assert
        Assert.False(res.Succeeded);
        Assert.Equal(400, res.StatusCode);
        _mockPowiadomieniaRepository.Verify(r => r.DeletePowiadomienie(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RozpatrzPowiadomienie_ProfileNotFound_DeletesNotificationAndReturnsNotFound()
    {
        // Arrange
        var userId = 7;
        var senderId = 8;
        var notif = StworzPowiadomienieEncje(3, (int)TypPowiadomieniaEnum.ZaproszenieDoZnajomych, uzytkownikId: userId, powiazanyId: senderId);
        _mockPowiadomieniaRepository.Setup(r => r.GetPowiadomienie(3)).ReturnsAsync(notif);
        _mockProfileService.Setup(p => p.GetProfil(userId)).ReturnsAsync(ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem("no profile")));
        _mockPowiadomieniaRepository.Setup(r => r.DeletePowiadomienie(3)).ReturnsAsync(true);

        // Act
        var res = await _service.RozpatrzPowiadomienie(3, true, userId);

        // Assert
        Assert.False(res.Succeeded);
        Assert.Equal(404, res.StatusCode);
        _mockPowiadomieniaRepository.Verify(r => r.DeletePowiadomienie(3), Times.Once);
    }
}

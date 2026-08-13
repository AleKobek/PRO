using Moq;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.Services;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Platformy.Services;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Uzytkownicy.Services;
using Squadra.Server.Modules.Wiadomosci.Services;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Services;
using Squadra.Server.Modules.Znajomosci.Services;
using Xunit.Abstractions;


namespace Squadra.Server.Tests.Services;

public class DruzynyServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IDruzynyRepository> _mockRepo = new();
    private readonly Mock<IWspieraneGryService> _mockGry = new();
    private readonly Mock<IUzytkownicyService> _mockUzytkownicy = new();
    private readonly Mock<IProfileService> _mockProfile = new();
    private readonly Mock<IJezykiService> _mockJezyki = new();
    private readonly Mock<IStopnieBieglosciJezykaService> _mockStopnie = new();
    private readonly Mock<IStatystykiService> _mockStat = new();
    private readonly Mock<IPlatformyService> _mockPlatformy = new();
    private readonly Mock<IBibliotekaGierService> _mockBiblioteka = new();
    private readonly Mock<IPowiadomieniaService> _mockPowiad = new();
    private readonly Mock<IZnajomosciService> _mockZnaj = new();
    private readonly Mock<IStatystykiCzatuService> _mockStatCzatu = new();

    public DruzynyServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private DruzynyService CreateService()
    {
        return new DruzynyService(
            _mockRepo.Object,
            _mockGry.Object,
            _mockUzytkownicy.Object,
            _mockProfile.Object,
            _mockJezyki.Object,
            _mockStopnie.Object,
            _mockStat.Object,
            _mockPlatformy.Object,
            _mockBiblioteka.Object,
            _mockPowiad.Object,
            _mockZnaj.Object,
            _mockStatCzatu.Object
        );
    }

    #region CzyUzytkownikSpelniaWymaganiaDruzyny

    [Fact]
    public async Task CzyUzytkownikSpelniaWymaganiaDruzyny_InvalidIds_ReturnsBadRequest()
    {
        var svc = CreateService();

        var res1 = await svc.CzyUzytkownikSpelniaWymaganiaDruzyny(0, 1);
        Assert.False(res1.Succeeded);
        Assert.Equal(400, res1.StatusCode);

        var res2 = await svc.CzyUzytkownikSpelniaWymaganiaDruzyny(1, 0);
        Assert.False(res2.Succeeded);
        Assert.Equal(400, res2.StatusCode);
    }

    [Fact]
    public async Task CzyUzytkownikSpelniaWymaganiaDruzyny_DruzynaNotFound_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetDruzyna(5)).ThrowsAsync(new NieZnalezionoWBazieException("brak"));
        var svc = CreateService();

        var res = await svc.CzyUzytkownikSpelniaWymaganiaDruzyny(5, 1);
        Assert.False(res.Succeeded);
        Assert.Equal(404, res.StatusCode);
    }

    [Fact]
    public async Task CzyUzytkownikSpelniaWymaganiaDruzyny_UserLacksGame_ReturnsFalse()
    {
        var dr = new Squadra.Server.Modules.Drużyny.Models.Druzyna { Id = 1, CzyZintegrowano = true, GraId = 10 };
        _mockRepo.Setup(r => r.GetDruzyna(1)).ReturnsAsync(dr);
        _mockBiblioteka.Setup(b => b.CzyUzytkownikMaDanaGre(2, 10)).ReturnsAsync(ServiceResult<bool>.Ok(false));

        var svc = CreateService();
        var res = await svc.CzyUzytkownikSpelniaWymaganiaDruzyny(1, 2);
        Assert.True(res.Succeeded);
        Assert.False(res.Value.CzySpelniaWymagania);
    }

    #endregion

    #region CzyUzytkownikSpelniaWymaganieMiejsca

    [Fact]
    public async Task CzyUzytkownikSpelniaWymaganieMiejsca_InvalidIds_ReturnsBadRequest()
    {
        var svc = CreateService();
        var res1 = await svc.CzyUzytkownikSpelniaWymaganieMiejsca(0, 1);
        Assert.False(res1.Succeeded);
        Assert.Equal(400, res1.StatusCode);

        var res2 = await svc.CzyUzytkownikSpelniaWymaganieMiejsca(1, 0);
        Assert.False(res2.Succeeded);
        Assert.Equal(400, res2.StatusCode);
    }

    [Fact]
    public async Task CzyUzytkownikSpelniaWymaganieMiejsca_RepoTrue_ReturnsTrue()
    {
        _mockRepo.Setup(r => r.CzyUzytkownikSpelniaWymaganieMiejsca(3, 4)).ReturnsAsync(true);
        var svc = CreateService();
        var res = await svc.CzyUzytkownikSpelniaWymaganieMiejsca(3, 4);
        Assert.True(res.Succeeded);
        Assert.True(res.Value);
    }

    [Fact]
    public async Task CzyUzytkownikSpelniaWymaganieMiejsca_RepoFalse_ReturnsFalse()
    {
        _mockRepo.Setup(r => r.CzyUzytkownikSpelniaWymaganieMiejsca(3, 5)).ReturnsAsync(false);
        var svc = CreateService();
        var res = await svc.CzyUzytkownikSpelniaWymaganieMiejsca(3, 5);
        Assert.True(res.Succeeded);
        Assert.False(res.Value);
    }

    #endregion

    #region StworzDruzyne

    [Fact]
    public async Task StworzDruzyne_InvalidName_ReturnsBadRequest()
    {
        var svc = CreateService();
        var req = new CreateDruzynaReqDto("", 1, true, null, 1, null, null, null, null, null, new List<CreateMiejsceWDruzynieReq>(), false);
        var res = await svc.StworzDruzyne(req, 1);
        Assert.False(res.Succeeded);
        Assert.Equal(400, res.StatusCode);
    }

    [Fact]
    public async Task StworzDruzyne_CaptainLacksRequiredLanguage_ReturnsBadRequest()
    {
        _mockGry.Setup(g => g.GetWspieranaGra(1)).ReturnsAsync(ServiceResult<WspieranaGraDto>.Ok(new WspieranaGraDto(1, "G", "")));
        _mockUzytkownicy.Setup(u => u.CzyUzytkownikMaZintegrowaneKonto(It.IsAny<int>())).ReturnsAsync(ServiceResult<bool>.Ok(false));
        _mockJezyki.Setup(j => j.GetJezyk(It.IsAny<int>())).ReturnsAsync(ServiceResult<JezykDto>.Ok(new JezykDto(1, "PL")));
        _mockStopnie.Setup(s => s.GetStopienBieglosciJezyka(It.IsAny<int>())).ReturnsAsync(ServiceResult<StopienBieglosciJezykaDto>.Ok(new StopienBieglosciJezykaDto(1, "Podstawowy", 1)));
        _mockRepo.Setup(r => r.CzyUzytkownikOsiagnalMaksLiczbeDruzyn(10, 1)).ReturnsAsync(false);
        // jezyki service returns no languages for captain
        _mockJezyki.Setup(j => j.GetJezykiProfiluZRownymiLubNizszymiStopniami(10)).ReturnsAsync(ServiceResult<ICollection<JezykOrazRowneLubNizszeStopnieDto>>.Ok(new List<JezykOrazRowneLubNizszeStopnieDto>()));
        
        var svc = CreateService();
        var req = new CreateDruzynaReqDto("Team", 1, true, null, 1, 1, 1, null, null, null, new List<CreateMiejsceWDruzynieReq>(), false);
        var res = await svc.StworzDruzyne(req, 10);
        Assert.False(res.Succeeded);
        Assert.Equal(400, res.StatusCode);
    }

    [Fact]
    public async Task StworzDruzyne_Success_ReturnsCreated()
    {
        var req = new CreateDruzynaReqDto("Team", 1, true, null, 1, 1, 2, null, null, null, new List<CreateMiejsceWDruzynieReq>(), false);
        var languages = new List<JezykOrazRowneLubNizszeStopnieDto> { new (new JezykDto(1, "PL"), new List<StopienBieglosciJezykaDto> {new (1, "Podstawowy", 1), new (2, "Średnio-zaawansowany", 2), new (3, "Zaawansowany", 3)}) };
        _mockGry.Setup(g => g.GetWspieranaGra(1)).ReturnsAsync(ServiceResult<WspieranaGraDto>.Ok(new WspieranaGraDto(1, "G", "")));
        _mockUzytkownicy.Setup(u => u.CzyUzytkownikMaZintegrowaneKonto(It.IsAny<int>())).ReturnsAsync(ServiceResult<bool>.Ok(false));
        _mockJezyki.Setup(j => j.GetJezyk(It.IsAny<int>())).ReturnsAsync(ServiceResult<JezykDto>.Ok(new JezykDto(1, "PL")));
        _mockStopnie.Setup(s => s.GetStopienBieglosciJezyka(It.IsAny<int>())).ReturnsAsync(ServiceResult<StopienBieglosciJezykaDto>.Ok(new StopienBieglosciJezykaDto(2, "Średnio-zaawansowany", 2)));
        _mockStat.Setup(s => s.CzyUzytkownikSpelniaWymagania(It.IsAny<ICollection<WartoscStatystykiDTO>>(), It.IsAny<int>())).ReturnsAsync(ServiceResult<bool>.Ok(true));
        _mockRepo.Setup(r => r.StworzDruzyne(req, 20)).ReturnsAsync(55);
        _mockRepo.Setup(r => r.CzyUzytkownikOsiagnalMaksLiczbeDruzyn(20, 1)).ReturnsAsync(false);
        _mockJezyki.Setup(j => j.GetJezykiProfiluZRownymiLubNizszymiStopniami(20)).ReturnsAsync(ServiceResult<ICollection<JezykOrazRowneLubNizszeStopnieDto>>.Ok(languages));
        
        var svc = CreateService();
        var res = await svc.StworzDruzyne(req, 20);
        Assert.True(res.Succeeded);
        Assert.Equal(201, res.StatusCode);
        Assert.Equal(55, res.Value);
    }

    #endregion

    #region WyszukajDruzyny

    [Fact]
    public async Task WyszukajDruzyny_UserNotIntegratedAndLookingForIntegrated_ReturnsBadRequest()
    {
        var svc = CreateService();
        var req = new WyszukajDruzyneReqDto(1, null, null, null, null, "zintegrowane", null,[]);
        _mockUzytkownicy.Setup(u => u.CzyUzytkownikMaZintegrowaneKonto(5)).ReturnsAsync(ServiceResult<bool>.Ok(false));
        var res = await svc.WyszukajDruzyny(req, 5);
        Assert.False(res.Succeeded);
        Assert.Equal(400, res.StatusCode);
    }

    [Fact]
    public async Task WyszukajDruzyny_GameNotFound_ReturnsFail()
    {
        var svc = CreateService();
        var req = new WyszukajDruzyneReqDto(2, null, null, null, null, "zintegrowane", null,[]);
        _mockUzytkownicy.Setup(u => u.CzyUzytkownikMaZintegrowaneKonto(1)).ReturnsAsync(ServiceResult<bool>.Ok(false));
        _mockGry.Setup(g => g.GetWspieranaGra(2)).ReturnsAsync(ServiceResult<WspieranaGraDto>.NotFound(new ErrorItem("no")));
        var res = await svc.WyszukajDruzyny(req, 1);
        Assert.False(res.Succeeded);
    }

    [Fact]
    public async Task WyszukajDruzyny_Success_ReturnsResults()
    {
        var req = new WyszukajDruzyneReqDto(1, null, null, null, null, "niezintegrowane", null,[]);
        _mockUzytkownicy.Setup(u => u.CzyUzytkownikMaZintegrowaneKonto(1)).ReturnsAsync(ServiceResult<bool>.Ok(false));
        _mockGry.Setup(g => g.GetWspieranaGra(1)).ReturnsAsync(ServiceResult<WspieranaGraDto>.Ok(new WspieranaGraDto(1, "Leczo PatPat Simulator", "clicker")));
        _mockJezyki.Setup(j => j.GetJezykiProfilu(1)).ReturnsAsync(ServiceResult<ICollection<JezykOrazStopienDto>>.Ok(new List<JezykOrazStopienDto>()));
        _mockRepo.Setup(r => r.WyszukajIdDruzyn(req, 1, It.IsAny<ICollection<JezykOrazStopienDto>>())).ReturnsAsync(new List<int> { 5 });
        _mockRepo.Setup(r => r.GetDruzyny(It.IsAny<int[]>())).ReturnsAsync(new List<Squadra.Server.Modules.Drużyny.Models.Druzyna> { new() { Id = 5, Nazwa = "T", GraId = 1, KapitanId = 1, CzyPubliczna = true, NastrojRozgrywkiId = 1 } });
        _mockRepo.Setup(r => r.GetMiejscaWDruzynie(5)).ReturnsAsync(new List<Squadra.Server.Modules.Drużyny.Models.MiejsceWDruzynie>());
        _mockStat.Setup(s => s.GetRoleGry(1)).ReturnsAsync(ServiceResult<ICollection<RolaDto>>.Ok(new List<RolaDto>()));
        _mockRepo.Setup(r => r.GetNastrojRozgrywki(It.IsAny<int>())).ReturnsAsync(new Modules.Drużyny.Models.NastrojRozgrywki { Id = 1, Nazwa = "N" });
        _mockRepo.Setup(r => r.GetDruzyna(It.IsAny<int>())).ReturnsAsync(new Squadra.Server.Modules.Drużyny.Models.Druzyna { Id = 5, Nazwa = "T", GraId = 1, KapitanId = 1, CzyPubliczna = true, NastrojRozgrywkiId = 1 });
        _mockUzytkownicy.Setup(u => u.GetOstatniaAktywnoscUzytkownika(It.IsAny<int>())).ReturnsAsync(ServiceResult<DateTime?>.Ok(DateTime.UtcNow));
        _mockRepo.Setup(r => r.GetDataOstatniegoOtwarciaCzatuUzytkownika(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((DateTime?)null);
        _mockStatCzatu.Setup(s => s.CzySaNoweWiadomosciWDruzynie(It.IsAny<int>(), It.IsAny<DateTime?>())).ReturnsAsync(ServiceResult<bool>.Ok(false));
        _mockProfile.Setup(p => p.GetProfilMinInfo(It.IsAny<int>())).ReturnsAsync(ServiceResult<ProfilMinInfoDto>.Ok(new ProfilMinInfoDto(1, "x", null, "Dostepny")));

        var svc = CreateService();
        var res = await svc.WyszukajDruzyny(req, 1);
        Assert.True(res.Succeeded);
    }

    #endregion

    #region DodajUzytkownikaNaMiejsce & ZaprosUzytkownikaNaMiejsce

    [Fact]
    public async Task DodajUzytkownikaNaMiejsce_InvalidIds_ReturnBadRequest()
    {
        var svc = CreateService();
        var r1 = await svc.DodajUzytkownikaNaMiejsce(0, 1);
        Assert.False(r1.Succeeded);
        Assert.Equal(400, r1.StatusCode);
        var r2 = await svc.DodajUzytkownikaNaMiejsce(1, 0);
        Assert.False(r2.Succeeded);
        Assert.Equal(400, r2.StatusCode);
    }

    [Fact]
    public async Task DodajUzytkownikaNaMiejsce_MiejsceZajete_ReturnsConflict()
    {
        _mockRepo.Setup(r => r.GetMiejsceWDruzynie(2)).ReturnsAsync(new Squadra.Server.Modules.Drużyny.Models.MiejsceWDruzynie { Id = 2, UzytkownikId = 5, DruzynaId = 3 });
        var svc = CreateService();
        var res = await svc.DodajUzytkownikaNaMiejsce(2, 4);
        Assert.False(res.Succeeded);
        Assert.Equal(409, res.StatusCode);
    }

    [Fact]
    public async Task DodajUzytkownikaNaMiejsce_Success_ReturnsNoContent()
    {
        var miejsce = new Squadra.Server.Modules.Drużyny.Models.MiejsceWDruzynie { Id = 8, UzytkownikId = null, DruzynaId = 7, RolaId = null };
        _mockRepo.Setup(r => r.GetMiejsceWDruzynie(8)).ReturnsAsync(miejsce);
        _mockRepo.Setup(r => r.CzyUzytkownikNalezyDoDruzyny(9, 7)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.DodajUzytkownikaNaMiejsce(8, 9)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.GetDruzyna(7)).ReturnsAsync(new Squadra.Server.Modules.Drużyny.Models.Druzyna { Id = 7, Nazwa = "T", KapitanId = 1, GraId = 1, CzyPubliczna = true });
        _mockRepo.Setup(r => r.CzyUzytkownikSpelniaWymaganieMiejsca(8, 9)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.CzyUzytkownikOsiagnalMaksLiczbeDruzyn(9, 1)).ReturnsAsync(false);
        _mockStat.Setup(s => s.GetRola(It.IsAny<int>())).ReturnsAsync(ServiceResult<RolaDto>.Ok(new RolaDto(1,"R",1)));
        _mockPowiad.Setup(p => p.WyslijPowiadomienieODolaczeniuDoDruzyny(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(ServiceResult<bool>.NoContent(true));
        _mockPowiad.Setup(p => p.CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(ServiceResult<bool>.Ok(false));
        _mockPowiad.Setup(p => p.DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(ServiceResult<bool>.Ok(true)));

        var svc = CreateService();
        var res = await svc.DodajUzytkownikaNaMiejsce(8, 9);
        _testOutputHelper.WriteLine($"DodajUzytkownikaNaMiejsce result: Succeeded={res.Succeeded}, StatusCode={res.StatusCode}");
        Assert.True(res.Succeeded);
        Assert.Equal(204, res.StatusCode);
    }

    [Fact]
    public async Task ZaprosUzytkownikaNaMiejsce_ValidationAndForbiddenAndSuccess()
    {
        var svc = CreateService();
        var rBad = await svc.ZaprosUzytkownikaNaMiejsce(0, 1, 2);
        Assert.False(rBad.Succeeded);

        // place taken
        _mockRepo.Setup(r => r.GetMiejsceWDruzynie(3)).ReturnsAsync(new Squadra.Server.Modules.Drużyny.Models.MiejsceWDruzynie { Id = 3, UzytkownikId = 4, DruzynaId = 1 });
        var rConflict = await svc.ZaprosUzytkownikaNaMiejsce(3, 5, 1);
        Assert.False(rConflict.Succeeded);

        // success
        _mockRepo.Setup(r => r.GetMiejsceWDruzynie(6)).ReturnsAsync(new Squadra.Server.Modules.Drużyny.Models.MiejsceWDruzynie { Id = 6, UzytkownikId = null, DruzynaId = 2 });
        _mockRepo.Setup(r => r.GetDruzyna(2)).ReturnsAsync(new Squadra.Server.Modules.Drużyny.Models.Druzyna { Id = 2, KapitanId = 8, Nazwa = "X", GraId = 1 });
        _mockRepo.Setup(r => r.CzyUzytkownikNalezyDoDruzyny(7, 2)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CzyUzytkownikOsiagnalMaksLiczbeDruzyn(7, 1)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CzyUzytkownikSpelniaWymaganeStatystykiDruzyny(2, 7)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.CzyUzytkownikSpelniaWymaganieMiejsca(6, 7)).ReturnsAsync(true);
        _mockPowiad.Setup(p => p.WyslijZaproszenieNaMiejsceWDruzynie(7, 2, "X", 6, null)).ReturnsAsync(ServiceResult<bool>.NoContent(true));

        var rOk = await svc.ZaprosUzytkownikaNaMiejsce(6, 7, 8);
        Assert.True(rOk.Succeeded);
        Assert.Equal(204, rOk.StatusCode);
    }

    #endregion
}


using Microsoft.EntityFrameworkCore;
using Moq;
using Squadra.Server.Context;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class ProfilRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IJezykRepository> _mockJezykRepository;
    private readonly Mock<IRegionRepository> _mockRegionRepository;
    private readonly Mock<IStatusRepository> _mockStatusRepository;
    private readonly ProfilRepository _repository;

    public ProfilRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockJezykRepository = new Mock<IJezykRepository>();
        _mockRegionRepository = new Mock<IRegionRepository>();
        _mockStatusRepository = new Mock<IStatusRepository>();
        _repository = new ProfilRepository(
            _context,
            _mockJezykRepository.Object,
            _mockRegionRepository.Object,
            _mockStatusRepository.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Profil.AddRange(
            new Profil
            {
                IdUzytkownika = 1,
                Pseudonim = "TestUser1",
                Zaimki = "he/him",
                Opis = "Test description",
                RegionId = 1,
                StatusId = 1,
                Awatar = new byte[] { 1, 2, 3 }
            },
            new Profil
            {
                IdUzytkownika = 2,
                Pseudonim = "TestUser2",
                Zaimki = null,
                Opis = null,
                RegionId = null,
                StatusId = 2,
                Awatar = Array.Empty<byte>()
            }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetProfile_ReturnsAllProfiles()
    {
        // Arrange
        var languages = new List<JezykOrazStopienDto>();
        var regionKraj = new RegionKrajDto(1, "Mazowieckie", 1, "Poland");
        var status = new StatusDto(1, "Online");

        _mockJezykRepository.Setup(r => r.GetJezykiProfilu(It.IsAny<int>()))
            .ReturnsAsync(languages);
        _mockRegionRepository.Setup(r => r.GetRegionIKraj(1))
            .ReturnsAsync(regionKraj);
        _mockRegionRepository.Setup(r => r.GetRegionIKraj(null))
            .ReturnsAsync((RegionKrajDto?)null);
        _mockStatusRepository.Setup(r => r.GetStatus(It.IsAny<int>()))
            .ReturnsAsync(status);

        // Act
        var result = await _repository.GetProfile();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Pseudonim == "TestUser1");
        Assert.Contains(result, p => p.Pseudonim == "TestUser2");
    }

    [Fact]
    public async Task GetProfilUzytkownika_WithValidId_ReturnsProfile()
    {
        // Arrange
        var userId = 1;
        var languages = new List<JezykOrazStopienDto>
        {
            new JezykOrazStopienDto(
                new JezykDto(1, "English"),
                new StopienBieglosciJezykaDto(3, "Advanced", 3))
        };
        var regionKraj = new RegionKrajDto(1, "Mazowieckie", 1, "Poland");
        var status = new StatusDto(1, "Online");

        _mockJezykRepository.Setup(r => r.GetJezykiProfilu(userId))
            .ReturnsAsync(languages);
        _mockRegionRepository.Setup(r => r.GetRegionIKraj(1))
            .ReturnsAsync(regionKraj);
        _mockStatusRepository.Setup(r => r.GetStatus(1))
            .ReturnsAsync(status);

        // Act
        var result = await _repository.GetProfilUzytkownika(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestUser1", result.Pseudonim);
        Assert.Equal("he/him", result.Zaimki);
        Assert.Equal("Test description", result.Opis);
        Assert.NotNull(result.RegionIKraj);
        Assert.Equal("Mazowieckie", result.RegionIKraj.NazwaRegionu);
        Assert.Single(result.Jezyki);
        Assert.Equal("Online", result.NazwaStatusu);
    }

    [Fact]
    public async Task GetProfilUzytkownika_WithNullRegion_ReturnsProfileWithNullRegion()
    {
        // Arrange
        var userId = 2;
        var languages = new List<JezykOrazStopienDto>();
        var status = new StatusDto(2, "Busy");

        _mockJezykRepository.Setup(r => r.GetJezykiProfilu(userId))
            .ReturnsAsync(languages);
        _mockRegionRepository.Setup(r => r.GetRegionIKraj(null))
            .ReturnsAsync((RegionKrajDto?)null);
        _mockStatusRepository.Setup(r => r.GetStatus(2))
            .ReturnsAsync(status);

        // Act
        var result = await _repository.GetProfilUzytkownika(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestUser2", result.Pseudonim);
        Assert.Null(result.Zaimki);
        Assert.Null(result.RegionIKraj);
    }

    [Fact]
    public async Task GetProfilUzytkownika_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetProfilUzytkownika(999));
    }

    [Fact]
    public async Task GetProfilUzytkownika_WhenStatusNotFound_ReturnsOfflineStatus()
    {
        // Arrange
        var userId = 1;
        var languages = new List<JezykOrazStopienDto>();
        var regionKraj = new RegionKrajDto(1, "Mazowieckie", 1, "Poland");
        var offlineStatus = new StatusDto(99, "Offline");

        _mockJezykRepository.Setup(r => r.GetJezykiProfilu(userId))
            .ReturnsAsync(languages);
        _mockRegionRepository.Setup(r => r.GetRegionIKraj(1))
            .ReturnsAsync(regionKraj);
        _mockStatusRepository.Setup(r => r.GetStatus(1))
            .ReturnsAsync((StatusDto?)null);
        _mockStatusRepository.Setup(r => r.GetStatusOffline())
            .Returns(offlineStatus);

        // Act
        var result = await _repository.GetProfilUzytkownika(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Offline", result.NazwaStatusu);
    }

    [Fact]
    public async Task UpdateProfil_WithValidData_UpdatesProfile()
    {
        // Arrange
        var userId = 1;
        var updateDto = new ProfilUpdateDto(
            2,
            "she/her",
            "Updated description",
            new List<JezykProfiluCreateDto>(),
            "UpdatedPseudonym"
        );
        _mockJezykRepository.Setup(r => r.ZmienJezykiProfilu(userId, updateDto.Jezyki))
            .ReturnsAsync(new List<JezykOrazStopienDto>());

        // Act
        var result = await _repository.UpdateProfil(userId, updateDto);

        // Assert
        Assert.True(result);
        var updatedProfile = await _context.Profil.FindAsync(userId);
        Assert.NotNull(updatedProfile);
        Assert.Equal("UpdatedPseudonym", updatedProfile.Pseudonim);
        Assert.Equal("she/her", updatedProfile.Zaimki);
        Assert.Equal("Updated description", updatedProfile.Opis);
        Assert.Equal(2, updatedProfile.RegionId);
        _mockJezykRepository.Verify(r => r.ZmienJezykiProfilu(userId, updateDto.Jezyki), Times.Once);
    }

    [Fact]
    public async Task UpdateProfil_WithInvalidId_ThrowsException()
    {
        // Arrange
        var updateDto = new ProfilUpdateDto(
            null,
            null,
            null,
            new List<JezykProfiluCreateDto>(),
            "Test"
        );

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.UpdateProfil(999, updateDto));
    }

    [Fact]
    public async Task UpdateAwatar_WithValidData_UpdatesAvatar()
    {
        // Arrange
        var userId = 1;
        var newAvatar = new byte[] { 10, 20, 30, 40 };

        // Act
        var result = await _repository.UpdateAwatar(userId, newAvatar);

        // Assert
        Assert.True(result);
        var updatedProfile = await _context.Profil.FindAsync(userId);
        Assert.NotNull(updatedProfile);
        Assert.Equal(newAvatar, updatedProfile.Awatar);
    }

    [Fact]
    public async Task UpdateAwatar_WithEmptyArray_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var originalAvatar = (await _context.Profil.FindAsync(userId))!.Awatar;
        var emptyAvatar = Array.Empty<byte>();

        // Act
        var result = await _repository.UpdateAwatar(userId, emptyAvatar);

        // Assert
        Assert.True(result);
        // Avatar should not be changed
        var profile = await _context.Profil.FindAsync(userId);
        Assert.Equal(originalAvatar, profile!.Awatar);
    }

    [Fact]
    public async Task UpdateAwatar_WithInvalidId_ThrowsException()
    {
        // Arrange
        var newAvatar = new byte[] { 1, 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.UpdateAwatar(999, newAvatar));
    }

    [Fact]
    public async Task CreateProfil_WithValidData_CreatesProfile()
    {
        // Arrange
        var createDto = new ProfilCreateReqDto(10, "NewUser");
        var languages = new List<JezykOrazStopienDto>();
        var status = new StatusDto(1, "Online");

        _mockJezykRepository.Setup(r => r.GetJezykiProfilu(10))
            .ReturnsAsync(languages);
        _mockRegionRepository.Setup(r => r.GetRegionIKraj(null))
            .ReturnsAsync((RegionKrajDto?)null);
        _mockStatusRepository.Setup(r => r.GetStatus(1))
            .ReturnsAsync(status);

        // Act
        var result = await _repository.CreateProfil(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewUser", result.Pseudonim);
        
        var createdProfile = await _context.Profil.FindAsync(10);
        Assert.NotNull(createdProfile);
        Assert.Equal(10, createdProfile.IdUzytkownika);
        Assert.Equal("NewUser", createdProfile.Pseudonim);
        Assert.Equal(1, createdProfile.StatusId);
        Assert.Null(createdProfile.RegionId);
    }

    [Fact]
    public async Task DeleteProfil_WithValidId_DeletesProfile()
    {
        // Arrange
        var userId = 1;
        _mockJezykRepository.Setup(r => r.ZmienJezykiProfilu(userId, It.IsAny<List<JezykProfiluCreateDto>>()))
            .ReturnsAsync(new List<JezykOrazStopienDto>());

        // Act
        await _repository.DeleteProfil(userId);

        // Assert
        var deletedProfile = await _context.Profil.FindAsync(userId);
        Assert.Null(deletedProfile);
        _mockJezykRepository.Verify(r => r.ZmienJezykiProfilu(userId, It.Is<List<JezykProfiluCreateDto>>(l => l.Count == 0)), Times.Once);
    }

    [Fact]
    public async Task DeleteProfil_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.DeleteProfil(999));
    }

    [Fact]
    public async Task GetStatusProfilu_WithValidId_ReturnsStatus()
    {
        // Arrange
        var userId = 1;
        var status = new StatusDto(1, "Online");
        _mockStatusRepository.Setup(r => r.GetStatus(1))
            .ReturnsAsync(status);

        // Act
        var result = await _repository.GetStatusProfilu(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Online", result.Nazwa);
    }

    [Fact]
    public async Task GetStatusProfilu_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            async () => await _repository.GetStatusProfilu(999));
    }

    [Fact]
    public async Task UpdateStatus_WithValidData_UpdatesStatus()
    {
        // Arrange
        var userId = 1;
        var newStatusId = 3;
        var newStatus = new StatusDto(3, "Do Not Disturb");
        _mockStatusRepository.Setup(r => r.GetStatus(newStatusId))
            .ReturnsAsync(newStatus);

        // Act
        var result = await _repository.UpdateStatus(userId, newStatusId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Do Not Disturb", result.Nazwa);
        
        var updatedProfile = await _context.Profil.FindAsync(userId);
        Assert.NotNull(updatedProfile);
        Assert.Equal(newStatusId, updatedProfile.StatusId);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidProfileId_ThrowsException()
    {
        // Arrange
        var newStatus = new StatusDto(2, "Busy");
        _mockStatusRepository.Setup(r => r.GetStatus(2))
            .ReturnsAsync(newStatus);

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.UpdateStatus(999, 2));
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidStatusId_ThrowsException()
    {
        // Arrange
        var userId = 1;
        _mockStatusRepository.Setup(r => r.GetStatus(999))
            .ReturnsAsync((StatusDto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.UpdateStatus(userId, 999));
    }
}

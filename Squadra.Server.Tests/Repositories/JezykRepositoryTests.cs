using Microsoft.EntityFrameworkCore;
using Moq;
using Squadra.Server.Context;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class JezykRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IStopienBieglosciJezykaRepository> _mockStopienRepository;
    private readonly JezykRepository _repository;

    public JezykRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockStopienRepository = new Mock<IStopienBieglosciJezykaRepository>();
        _repository = new JezykRepository(_context, _mockStopienRepository.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Jezyk.AddRange(
            new Jezyk { Id = 1, Nazwa = "English" },
            new Jezyk { Id = 2, Nazwa = "Polish" },
            new Jezyk { Id = 3, Nazwa = "German" }
        );

        _context.Profil.AddRange(
            new Profil { IdUzytkownika = 1, Pseudonim = "User1", StatusId = 1 },
            new Profil { IdUzytkownika = 2, Pseudonim = "User2", StatusId = 1 }
        );

        _context.JezykProfilu.AddRange(
            new JezykProfilu { UzytkownikId = 1, JezykId = 1, StopienBieglosciId = 3 },
            new JezykProfilu { UzytkownikId = 1, JezykId = 2, StopienBieglosciId = 5 }
        );

        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetJezyki_ReturnsAllLanguages()
    {
        // Act
        var result = await _repository.GetJezyki();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, j => j.Nazwa == "English");
        Assert.Contains(result, j => j.Nazwa == "Polish");
        Assert.Contains(result, j => j.Nazwa == "German");
    }

    [Fact]
    public async Task GetJezyk_WithValidId_ReturnsLanguage()
    {
        // Act
        var result = await _repository.GetJezyk(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("English", result.Nazwa);
    }

    [Fact]
    public async Task GetJezyk_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetJezyk(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetJezykiProfilu_WithValidUserId_ReturnsUserLanguages()
    {
        // Arrange
        var stopnieBieglosci = new List<StopienBieglosciJezykaDto>
        {
            new StopienBieglosciJezykaDto(3, "Advanced", 3),
            new StopienBieglosciJezykaDto(5, "Native", 5)
        };
        _mockStopienRepository.Setup(r => r.GetStopnieBieglosciJezyka())
            .ReturnsAsync(stopnieBieglosci);

        // Act
        var result = await _repository.GetJezykiProfilu(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, j => j.Jezyk.Nazwa == "English");
        Assert.Contains(result, j => j.Jezyk.Nazwa == "Polish");
    }

    [Fact]
    public async Task GetJezykiProfilu_WithUserWithNoLanguages_ReturnsEmptyList()
    {
        // Arrange
        _mockStopienRepository.Setup(r => r.GetStopnieBieglosciJezyka())
            .ReturnsAsync(new List<StopienBieglosciJezykaDto>());

        // Act
        var result = await _repository.GetJezykiProfilu(2);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ZmienJezykiProfilu_WithValidData_UpdatesLanguages()
    {
        // Arrange
        var noweJezyki = new List<JezykProfiluCreateDto>
        {
            new JezykProfiluCreateDto(3, 2) // German, Intermediate
        };
        var stopnieBieglosci = new List<StopienBieglosciJezykaDto>
        {
            new StopienBieglosciJezykaDto(2, "Intermediate", 2)
        };
        _mockStopienRepository.Setup(r => r.GetStopnieBieglosciJezyka())
            .ReturnsAsync(stopnieBieglosci);

        // Act
        var result = await _repository.ZmienJezykiProfilu(1, noweJezyki);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("German", result.First().Jezyk.Nazwa);
        
        // Verify old languages were removed
        var remainingLanguages = await _context.JezykProfilu.Where(jp => jp.UzytkownikId == 1).ToListAsync();
        Assert.Single(remainingLanguages);
        Assert.Equal(3, remainingLanguages[0].JezykId);
    }

    [Fact]
    public async Task ZmienJezykiProfilu_WithInvalidProfileId_ThrowsException()
    {
        // Arrange
        var noweJezyki = new List<JezykProfiluCreateDto>();

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.ZmienJezykiProfilu(999, noweJezyki));
    }

    [Fact]
    public async Task ZmienJezykiProfilu_WithEmptyList_RemovesAllLanguages()
    {
        // Arrange
        var noweJezyki = new List<JezykProfiluCreateDto>();
        _mockStopienRepository.Setup(r => r.GetStopnieBieglosciJezyka())
            .ReturnsAsync(new List<StopienBieglosciJezykaDto>());

        // Act
        var result = await _repository.ZmienJezykiProfilu(1, noweJezyki);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        
        // Verify all languages were removed
        var remainingLanguages = await _context.JezykProfilu.Where(jp => jp.UzytkownikId == 1).ToListAsync();
        Assert.Empty(remainingLanguages);
    }
}

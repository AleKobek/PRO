using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class StopienBieglosciJezykaRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly StopienBieglosciJezykaRepository _repository;

    public StopienBieglosciJezykaRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new StopienBieglosciJezykaRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.StopienBieglosciJezyka.AddRange(
            new StopienBieglosciJezyka { Id = 1, Nazwa = "Podstawowy", Wartosc = 1 },
            new StopienBieglosciJezyka { Id = 2, Nazwa = "Średnio-zaawansowany", Wartosc = 2 },
            new StopienBieglosciJezyka { Id = 3, Nazwa = "Zaawansowany", Wartosc = 3 }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetStopnieBieglosciJezyka_ReturnsAllProficiencyLevels()
    {
        // Act
        var result = await _repository.GetStopnieBieglosciJezyka();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, s => s.Nazwa == "Podstawowy" && s.Wartosc == 1);
        Assert.Contains(result, s => s.Nazwa == "Zaawansowany" && s.Wartosc == 3);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithValidId_ReturnsProficiencyLevel()
    {
        // Act
        var result = await _repository.GetStopienBieglosciJezyka(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Średnio-zaawansowany", result.Nazwa);
        Assert.Equal(2, result.Wartosc);
    }

    [Fact]
    public async Task GetStopienBieglosciJezyka_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetStopienBieglosciJezyka(999);

        // Assert
        Assert.Null(result);
    }
}

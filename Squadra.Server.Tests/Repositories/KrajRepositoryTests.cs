using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class KrajRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly KrajRepository _repository;

    public KrajRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new KrajRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Kraj.AddRange(
            new Kraj { Id = 1, Nazwa = "Poland" },
            new Kraj { Id = 2, Nazwa = "Germany" },
            new Kraj { Id = 3, Nazwa = "France" }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetKraje_ReturnsAllCountries()
    {
        // Act
        var result = await _repository.GetKraje();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, k => k.Nazwa == "Poland");
        Assert.Contains(result, k => k.Nazwa == "Germany");
        Assert.Contains(result, k => k.Nazwa == "France");
    }

    [Fact]
    public async Task GetKraj_WithValidId_ReturnsCountry()
    {
        // Act
        var result = await _repository.GetKraj(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Poland", result.Nazwa);
    }

    [Fact]
    public async Task GetKraj_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _repository.GetKraj(999));
    }

    [Fact]
    public async Task GetKraj_WithIdLessThanOne_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _repository.GetKraj(0));
        Assert.Contains("nie istnieje", exception.Message);
    }

    [Fact]
    public void GetKrajDomyslny_ReturnsDefaultCountry()
    {
        // Act
        var result = _repository.GetKrajDomyslny();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Unknown", result.Nazwa);
    }
}

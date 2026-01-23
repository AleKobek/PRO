using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class StatusRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly StatusRepository _repository;

    public StatusRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new StatusRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Status.AddRange(
            new Status { Id = 1, Nazwa = "Dostępny" },
            new Status { Id = 2, Nazwa = "Zaraz wracam" },
            new Status { Id = 3, Nazwa = "Nie przeszkadzać" }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetStatusy_ReturnsAllStatuses()
    {
        // Act
        var result = await _repository.GetStatusy();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, s => s.Nazwa == "Dostępny");
        Assert.Contains(result, s => s.Nazwa == "Zaraz wracam");
    }

    [Fact]
    public async Task GetStatus_ById_WithValidId_ReturnsStatus()
    {
        // Act
        var result = await _repository.GetStatus(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Dostępny", result.Nazwa);
    }

    [Fact]
    public async Task GetStatus_ById_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetStatus(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStatus_ByName_WithValidName_ReturnsStatus()
    {
        // Act
        var result = await _repository.GetStatus("Dostępny");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Dostępny", result.Nazwa);
    }

    [Fact]
    public async Task GetStatus_ByName_WithInvalidName_ReturnsNull()
    {
        // Act
        var result = await _repository.GetStatus("NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetIdStatusu_WithValidName_ReturnsId()
    {
        // Act
        var result = await _repository.GetIdStatusu("Dostępny");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetIdStatusu_WithInvalidName_ReturnsNull()
    {
        // Act
        var result = await _repository.GetIdStatusu("NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetStatusOffline_ReturnsOfflineStatus()
    {
        // Act
        var result = _repository.GetStatusOffline();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Id); // Max ID (3) + 1
        Assert.Equal("Offline", result.Nazwa);
    }
}

using Microsoft.EntityFrameworkCore;
using Moq;
using Squadra.Server.Context;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class RegionRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IKrajRepository> _mockKrajRepository;
    private readonly RegionRepository _repository;

    public RegionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockKrajRepository = new Mock<IKrajRepository>();
        _repository = new RegionRepository(_context, _mockKrajRepository.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Region.AddRange(
            new Region { Id = 1, KrajId = 1, Nazwa = "Mazowieckie" },
            new Region { Id = 2, KrajId = 1, Nazwa = "Małopolskie" },
            new Region { Id = 3, KrajId = 2, Nazwa = "Bavaria" }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetRegiony_ReturnsAllRegions()
    {
        // Act
        var result = await _repository.GetRegiony();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.Nazwa == "Mazowieckie");
        Assert.Contains(result, r => r.Nazwa == "Bavaria");
    }

    [Fact]
    public async Task GetRegion_WithValidId_ReturnsRegion()
    {
        // Act
        var result = await _repository.GetRegion(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Mazowieckie", result.Nazwa);
        Assert.Equal(1, result.KrajId);
    }

    [Fact]
    public async Task GetRegion_WithNullId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetRegion(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRegion_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetRegion(999));
    }

    [Fact]
    public async Task GetRegionIKraj_WithValidId_ReturnsRegionWithCountry()
    {
        // Arrange
        _mockKrajRepository.Setup(r => r.GetKraj(1))
            .ReturnsAsync(new KrajDto(1, "Poland"));

        // Act
        var result = await _repository.GetRegionIKraj(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.IdRegionu);
        Assert.Equal("Mazowieckie", result.NazwaRegionu);
        Assert.Equal(1, result.IdKraju);
        Assert.Equal("Poland", result.NazwaKraju);
    }

    [Fact]
    public async Task GetRegionIKraj_WithNullId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetRegionIKraj(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRegionIKraj_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetRegionIKraj(999));
    }

    [Fact]
    public async Task GetRegionyKraju_WithValidCountryId_ReturnsRegions()
    {
        // Act
        var result = await _repository.GetRegionyKraju(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(1, r.KrajId));
    }

    [Fact]
    public async Task GetRegionyKraju_WithInvalidCountryId_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetRegionyKraju(999));
        Assert.Contains("nie istnieje", exception.Message);
    }
}

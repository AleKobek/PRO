using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Wiadomosci.DTO;
using Squadra.Server.Modules.Wiadomosci.Models;
using Squadra.Server.Modules.Wiadomosci.Repositories;

namespace Squadra.Server.Tests.Repositories;

public class WiadomoscRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly WiadomoscRepository _repository;

    public WiadomoscRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new WiadomoscRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var now = DateTime.Now;

        _context.Wiadomosc.AddRange(
            new Wiadomosc
            {
                Id = 1,
                IdNadawcy = 1,
                IdOdbiorcy = 2,
                DataWyslania = now.AddHours(-2),
                Tresc = "Hello from user 1",
                IdTypuWiadomosci = 1
            },
            new Wiadomosc
            {
                Id = 2,
                IdNadawcy = 2,
                IdOdbiorcy = 1,
                DataWyslania = now.AddHours(-1),
                Tresc = "Reply from user 2",
                IdTypuWiadomosci = 1
            },
            new Wiadomosc
            {
                Id = 3,
                IdNadawcy = 1,
                IdOdbiorcy = 3,
                DataWyslania = now,
                Tresc = "Message to user 3",
                IdTypuWiadomosci = 1
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
    public async Task GetWiadomosc_WithValidId_ReturnsMessage()
    {
        // Act
        var result = await _repository.GetWiadomosc(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.IdNadawcy);
        Assert.Equal(2, result.IdOdbiorcy);
        Assert.Equal("Hello from user 1", result.Tresc);
        Assert.Matches(@"^\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}$", result.DataWyslania);
    }

    [Fact]
    public async Task GetWiadomosc_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            async () => await _repository.GetWiadomosc(999));
    }

    [Fact]
    public async Task GetWiadomosci_BetweenTwoUsers_ReturnsConversationSortedAscending()
    {
        // Act
        var result = await _repository.GetWiadomosci(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Collection(
            result,
            first => Assert.Equal("Hello from user 1", first.Tresc),
            second => Assert.Equal("Reply from user 2", second.Tresc));
    }

    [Fact]
    public async Task GetWiadomosci_NoConversation_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetWiadomosci(1, 999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateWiadomosc_WithValidData_ReturnsTrue()
    {
        // Arrange
        var dto = new WiadomoscCreateDto("New message", 1);
        var senderId = 1;
        var receiverId = 3;

        // Act
        var result = await _repository.CreateWiadomosc(receiverId, dto, senderId);

        // Assert
        Assert.True(result);
        var messages = await _context.Wiadomosc.Where(m => m.Tresc == "New message").ToListAsync();
        Assert.Single(messages);
        Assert.Equal(senderId, messages[0].IdNadawcy);
        Assert.Equal(receiverId, messages[0].IdOdbiorcy);
        Assert.Equal(1, messages[0].IdTypuWiadomosci);
    }

    [Fact]
    public async Task DeleteWiadomosciUzytkownikow_RemovesAllMessagesBetweenUsers()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;

        // Act
        var result = await _repository.DeleteWiadomosciUzytkownikow(userId1, userId2);

        // Assert
        Assert.True(result);
        var remainingMessages = await _repository.GetWiadomosci(userId1, userId2);
        Assert.Empty(remainingMessages);

        // Verify message to user 3 is still there
        var messageToUser3 = await _repository.GetWiadomosc(3);
        Assert.NotNull(messageToUser3);
    }
}

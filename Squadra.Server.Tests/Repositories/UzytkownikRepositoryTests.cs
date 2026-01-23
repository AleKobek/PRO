using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Squadra.Server.Context;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Xunit;

namespace Squadra.Server.Tests.Repositories;

public class UzytkownikRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UzytkownikRepository _repository;
    private readonly Mock<IProfilRepository> _mockProfilRepository;
    private readonly Mock<IPowiadomienieRepository> _mockPowiadomienieRepository;
    private readonly Mock<IZnajomiRepository> _mockZnajomiRepository;
    private readonly UserManager<Uzytkownik> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UzytkownikRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Mock dependencies
        _mockProfilRepository = new Mock<IProfilRepository>();
        _mockPowiadomienieRepository = new Mock<IPowiadomienieRepository>();
        _mockZnajomiRepository = new Mock<IZnajomiRepository>();

        // Create real UserManager and RoleManager for InMemory testing
        _userManager = CreateUserManager();
        _roleManager = CreateRoleManager();

        _repository = new UzytkownikRepository(
            _context,
            _mockProfilRepository.Object,
            _mockPowiadomienieRepository.Object,
            _mockZnajomiRepository.Object,
            _userManager,
            _roleManager
        );

        SeedTestData().Wait();
    }

    private UserManager<Uzytkownik> CreateUserManager()
    {
        var store = new Mock<IUserStore<Uzytkownik>>();
        var userManager = new Mock<UserManager<Uzytkownik>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<Uzytkownik>>().Object,
            Array.Empty<IUserValidator<Uzytkownik>>(),
            Array.Empty<IPasswordValidator<Uzytkownik>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<Uzytkownik>>>().Object
        );

        // Setup common UserManager methods
        userManager.Setup(um => um.GetRolesAsync(It.IsAny<Uzytkownik>()))
            .ReturnsAsync(new List<string> { "Uzytkownik" });

        return userManager.Object;
    }

    private RoleManager<IdentityRole<int>> CreateRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole<int>>>();
        var roleManager = new Mock<RoleManager<IdentityRole<int>>>(
            store.Object,
            Array.Empty<IRoleValidator<IdentityRole<int>>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole<int>>>>().Object
        );

        roleManager.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        return roleManager.Object;
    }

    private async Task SeedTestData()
    {
        var user1 = new Uzytkownik
        {
            Id = 1,
            UserName = "testuser1",
            NormalizedUserName = "TESTUSER1",
            Email = "test1@example.com",
            NormalizedEmail = "TEST1@EXAMPLE.COM",
            PhoneNumber = "123456789",
            DataUrodzenia = new DateOnly(1990, 1, 1),
            OstatniaAktywnosc = new DateTime(2024, 1, 1)
        };

        var user2 = new Uzytkownik
        {
            Id = 2,
            UserName = "testuser2",
            NormalizedUserName = "TESTUSER2",
            Email = "test2@example.com",
            NormalizedEmail = "TEST2@EXAMPLE.COM",
            PhoneNumber = "987654321",
            DataUrodzenia = new DateOnly(1995, 6, 15),
            OstatniaAktywnosc = new DateTime(2024, 1, 2)
        };

        _context.Uzytkownik.AddRange(user1, user2);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUzytkownicy_ReturnsAllUsers()
    {
        // Act
        var result = await _repository.GetUzytkownicy();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Login == "testuser1");
        Assert.Contains(result, u => u.Login == "testuser2");
    }

    [Fact]
    public async Task GetUzytkownik_ByValidId_ReturnsUser()
    {
        // Act
        var result = await _repository.GetUzytkownik(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser1", result.Login);
        Assert.Equal("test1@example.com", result.Email);
        Assert.Equal("123456789", result.NumerTelefonu);
        Assert.Equal(new DateOnly(1990, 1, 1), result.DataUrodzenia);
    }

    [Fact]
    public async Task GetUzytkownik_ByInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            () => _repository.GetUzytkownik(999)
        );
    }

    [Fact]
    public async Task GetUzytkownik_ByValidLogin_ReturnsUser()
    {
        // Act
        var result = await _repository.GetUzytkownik("testuser1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser1", result.Login);
        Assert.Equal("test1@example.com", result.Email);
    }

    [Fact]
    public async Task GetUzytkownik_ByInvalidLogin_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            () => _repository.GetUzytkownik("nonexistent")
        );
    }

    [Fact]
    public async Task GetOstatniaAktywnoscUzytkownika_WithValidId_ReturnsActivity()
    {
        // Act
        var result = await _repository.GetOstatniaAktywnoscUzytkownika(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTime(2024, 1, 1), result.Value);
    }

    [Fact]
    public async Task GetOstatniaAktywnoscUzytkownika_WithInvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            () => _repository.GetOstatniaAktywnoscUzytkownika(999)
        );
    }

    [Fact]
    public async Task CzyLoginIstnieje_WithExistingLogin_ReturnsTrue()
    {
        // Act
        var result = await _repository.CzyLoginIstnieje(null, "testuser1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CzyLoginIstnieje_WithNonExistingLogin_ReturnsFalse()
    {
        // Act
        var result = await _repository.CzyLoginIstnieje(null, "newuser");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CzyLoginIstnieje_WithSameUserLogin_ReturnsFalse()
    {
        // User 1 checking their own login should return false (not considered duplicate)
        // Act
        var result = await _repository.CzyLoginIstnieje(1, "testuser1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CzyLoginIstnieje_CaseInsensitive_ReturnsTrue()
    {
        // Act
        var result = await _repository.CzyLoginIstnieje(null, "TESTUSER1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CzyEmailIstnieje_WithExistingEmail_ReturnsTrue()
    {
        // Act
        var result = await _repository.CzyEmailIstnieje(null, "test1@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CzyEmailIstnieje_WithNonExistingEmail_ReturnsFalse()
    {
        // Act
        var result = await _repository.CzyEmailIstnieje(null, "new@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CzyEmailIstnieje_WithSameUserEmail_ReturnsFalse()
    {
        // User 1 checking their own email should return false (not considered duplicate)
        // Act
        var result = await _repository.CzyEmailIstnieje(1, "test1@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CzyEmailIstnieje_CaseInsensitive_ReturnsTrue()
    {
        // Act
        var result = await _repository.CzyEmailIstnieje(null, "TEST1@EXAMPLE.COM");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateUzytkownik_WithValidData_UpdatesUser()
    {
        // Arrange
        var updateDto = new UzytkownikUpdateDto(
            "updateduser",
            "updated@example.com",
            "111222333",
            new DateOnly(1990, 1, 1)
        );

        // Act
        var result = await _repository.UpdateUzytkownik(1, updateDto);

        // Assert
        Assert.True(result);
        var updatedUser = await _context.Uzytkownik.FindAsync(1);
        Assert.Equal("updateduser", updatedUser.UserName);
        Assert.Equal("UPDATEDUSER", updatedUser.NormalizedUserName);
        Assert.Equal("updated@example.com", updatedUser.Email);
        Assert.Equal("UPDATED@EXAMPLE.COM", updatedUser.NormalizedEmail);
        Assert.Equal("111222333", updatedUser.PhoneNumber);
    }

    [Fact]
    public async Task UpdateUzytkownik_WithInvalidId_ThrowsException()
    {
        // Arrange
        var updateDto = new UzytkownikUpdateDto(
            "updateduser",
            "updated@example.com",
            "111222333",
            new DateOnly(1990, 1, 1)
        );

        // Act & Assert
        await Assert.ThrowsAsync<NieZnalezionoWBazieException>(
            () => _repository.UpdateUzytkownik(999, updateDto)
        );
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _userManager.Dispose();
        _roleManager.Dispose();
    }
}

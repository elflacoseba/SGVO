using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Services;
using SGVO.Shared;

namespace SGVO.UnitTests.Services;

/// <summary>
/// Integration tests for AuthService using EF Core InMemory.
/// Exercises the full login/refresh/logout flow to catch runtime errors
/// that unit tests with mocks would miss.
/// </summary>
public class AuthServiceTests
{
    private readonly SgvoDbContext _dbContext;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<SgvoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SgvoDbContext(options);
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _configMock = new Mock<IConfiguration>();

        _tokenServiceMock
            .Setup(t => t.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns((long id, string user, IEnumerable<string> roles) =>
                ("fake-jwt-token-for-user-" + id, DateTime.UtcNow.AddMinutes(15)));

        _passwordHasherMock
            .Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _configMock.Setup(c => c["Jwt:RefreshTokenExpirationDays"]).Returns("7");

        _authService = new AuthService(_dbContext, _tokenServiceMock.Object, _passwordHasherMock.Object, _configMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new UsuarioEntity
        {
            Id = 1,
            NombreUsuario = "testuser",
            PasswordHash = "hashed-password",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        var role = new RoleEntity
        {
            Id = 1,
            Nombre = "Admin",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        var userRole = new UsuarioRoleEntity
        {
            UsuarioId = 1,
            RolId = 1
        };

        _dbContext.Usuarios.Add(user);
        _dbContext.Roles.Add(role);
        _dbContext.UsuarioRoles.Add(userRole);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        var result = await _authService.LoginAsync("testuser", "password");

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("fake-jwt-token-for-user-1");
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUser_ShouldFail()
    {
        var result = await _authService.LoginAsync("nonexistent", "password");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Credenciales inválidas.");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // First login to create a refresh token
        var loginResult = await _authService.LoginAsync("testuser", "password");
        loginResult.IsSuccess.Should().BeTrue();

        var refreshToken = loginResult.Value.RefreshToken;

        // Now refresh
        var refreshResult = await _authService.RefreshTokenAsync(refreshToken);

        refreshResult.IsSuccess.Should().BeTrue();
        refreshResult.Value.AccessToken.Should().Be("fake-jwt-token-for-user-1");
        refreshResult.Value.RefreshToken.Should().NotBeEmpty();
        refreshResult.Value.RefreshToken.Should().NotBe(refreshToken); // Should be a new token
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldFail()
    {
        var result = await _authService.RefreshTokenAsync("invalid-token");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Refresh token inválido.");
    }

    [Fact]
    public async Task LogoutAsync_WithValidToken_ShouldSucceed()
    {
        var loginResult = await _authService.LoginAsync("testuser", "password");
        var refreshToken = loginResult.Value.RefreshToken;

        var result = await _authService.LogoutAsync(1, refreshToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task LogoutAsync_WithInvalidToken_ShouldFail()
    {
        var result = await _authService.LogoutAsync(1, "invalid-token");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Refresh token no encontrado.");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUser_ShouldReturnUser()
    {
        var result = await _authService.GetUserByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1L);
        result.Value.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistingUser_ShouldFail()
    {
        var result = await _authService.GetUserByIdAsync(999);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Usuario no encontrado.");
    }
}

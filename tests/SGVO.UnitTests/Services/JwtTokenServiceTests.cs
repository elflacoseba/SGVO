using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SGVO.Infrastructure.Services;

namespace SGVO.UnitTests.Services;

/// <summary>
/// Tests unitarios para JwtTokenService.
/// </summary>
public class JwtTokenServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<JwtTokenService>> _loggerMock;
    private readonly JwtTokenService _service;
    private const string Secret = "this-is-a-valid-secret-key-for-testing-purposes-only!";

    public JwtTokenServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<JwtTokenService>>();
        _configMock.Setup(c => c["Jwt:Secret"]).Returns(Secret);
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        _configMock.Setup(c => c["Jwt:AccessTokenExpirationMinutes"]).Returns("15");

        _service = new JwtTokenService(_configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnTokenAndExpiresAt()
    {
        var roles = new[] { "Admin", "User" };

        var (token, expiresAt) = _service.GenerateAccessToken(1, "admin", roles);

        token.Should().NotBeNullOrEmpty();
        expiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeUidClaim()
    {
        var (token, _) = _service.GenerateAccessToken(42, "testuser", Array.Empty<string>());

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var uidClaim = jwt.Claims.First(c => c.Type == "uid");

        uidClaim.Value.Should().Be("42");
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeRoleClaims()
    {
        var roles = new[] { "Admin", "Editor" };

        var (token, _) = _service.GenerateAccessToken(1, "user", roles);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        roleClaims.Should().HaveCount(2);
        roleClaims.Select(c => c.Value).Should().Contain("Admin");
        roleClaims.Select(c => c.Value).Should().Contain("Editor");
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnUserId()
    {
        var (token, _) = _service.GenerateAccessToken(123, "user", Array.Empty<string>());

        var result = _service.ValidateToken(token);

        result.Should().Be(123L);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        var result = _service.ValidateToken("invalid-token-string");

        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithEmptyString_ShouldReturnNull()
    {
        var result = _service.ValidateToken("");

        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithTamperedToken_ShouldReturnNull()
    {
        var (token, _) = _service.GenerateAccessToken(1, "user", Array.Empty<string>());
        var tampered = token + "x";

        var result = _service.ValidateToken(tampered);

        result.Should().BeNull();
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowWhenSecretMissing()
    {
        _configMock.Setup(c => c["Jwt:Secret"]).Returns((string?)null);
        var svc = new JwtTokenService(_configMock.Object, _loggerMock.Object);

        var act = () => svc.GenerateAccessToken(1, "user", Array.Empty<string>());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT Secret*");
    }
}

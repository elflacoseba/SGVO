using FluentAssertions;
using SGVO.Application.Features.Auth;
using SGVO.Application.Features.Auth.Dtos;

namespace SGVO.UnitTests.Features.Auth;

/// <summary>
/// Tests unitarios para LoginResponseMapper.
/// </summary>
public class LoginResponseMapperTests
{
    [Fact]
    public void Map_ShouldReturnCorrectDto()
    {
        var expiresAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = LoginResponseMapper.Map("access-token", "refresh-token", expiresAt);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.ExpiresAt.Should().Be(expiresAt);
        result.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public void Map_ShouldPreserveUtcDateTime()
    {
        var now = DateTime.UtcNow;

        var result = LoginResponseMapper.Map("token", "refresh", now);

        result.ExpiresAt.Kind.Should().Be(DateTimeKind.Utc);
        result.ExpiresAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
    }
}

using System.Security.Claims;
using FluentAssertions;
using SGVO.Api.Extensions;

namespace SGVO.IntegrationTests.Extensions;

/// <summary>
/// Tests for ClaimsPrincipalExtensions.GetUserId().
/// </summary>
public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_WithUidClaim_ShouldReturnId()
    {
        var claims = new[] { new Claim("uid", "42") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        result.Should().Be(42L);
    }

    [Fact]
    public void GetUserId_WithNameIdentifierClaim_ShouldReturnId()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "99") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        result.Should().Be(99L);
    }

    [Fact]
    public void GetUserId_WithBothClaims_ShouldPreferUid()
    {
        var claims = new[]
        {
            new Claim("uid", "42"),
            new Claim(ClaimTypes.NameIdentifier, "99")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        result.Should().Be(42L);
    }

    [Fact]
    public void GetUserId_WithNoClaims_ShouldReturnNull()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserId_WithInvalidUid_ShouldReturnNull()
    {
        var claims = new[] { new Claim("uid", "not-a-number") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserId_WithZeroId_ShouldReturnZero()
    {
        var claims = new[] { new Claim("uid", "0") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        result.Should().Be(0L);
    }
}

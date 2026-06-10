using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.UnitTests.Persistence;

/// <summary>
/// Tests that verify EF Core model configuration to catch type mapping issues
/// before they manifest as runtime errors with the MySQL provider.
/// </summary>
public class RefreshTokenConfigurationTests
{
    private static IModel GetModel()
    {
        var options = new DbContextOptionsBuilder<SgvoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new SgvoDbContext(options);
        return context.Model;
    }

    [Fact]
    public void RefreshToken_FamilyId_ShouldNotBeMappedAsGuid()
    {
        var model = GetModel();
        var entityType = model.FindEntityType(typeof(RefreshTokenEntity))!;
        var property = entityType.FindProperty(nameof(RefreshTokenEntity.FamilyId))!;

        // FamilyId is a string property - verify CLR type is string, not Guid
        property.ClrType.Should().Be(typeof(string));
    }

    [Fact]
    public void RefreshToken_TokenHash_ShouldNotBeMappedAsGuid()
    {
        var model = GetModel();
        var entityType = model.FindEntityType(typeof(RefreshTokenEntity))!;
        var property = entityType.FindProperty(nameof(RefreshTokenEntity.TokenHash))!;

        property.ClrType.Should().Be(typeof(string));
    }

    [Fact]
    public void RefreshToken_TokenHash_ShouldHaveCorrectMaxLength()
    {
        var model = GetModel();
        var entityType = model.FindEntityType(typeof(RefreshTokenEntity))!;
        var property = entityType.FindProperty(nameof(RefreshTokenEntity.TokenHash))!;

        property.GetMaxLength().Should().Be(64);
    }

    [Fact]
    public void RefreshToken_FamilyId_ShouldHaveCorrectMaxLength()
    {
        var model = GetModel();
        var entityType = model.FindEntityType(typeof(RefreshTokenEntity))!;
        var property = entityType.FindProperty(nameof(RefreshTokenEntity.FamilyId))!;

        property.GetMaxLength().Should().Be(36);
    }

    [Fact]
    public void RefreshToken_TokenHash_ShouldHaveUniqueIndex()
    {
        var model = GetModel();
        var entityType = model.FindEntityType(typeof(RefreshTokenEntity))!;
        var indexes = entityType.GetIndexes();

        var tokenHashIndex = indexes.FirstOrDefault(i =>
            i.Properties.Count == 1 &&
            i.Properties[0].Name == nameof(RefreshTokenEntity.TokenHash));

        tokenHashIndex.Should().NotBeNull();
        tokenHashIndex!.IsUnique.Should().BeTrue();
    }
}

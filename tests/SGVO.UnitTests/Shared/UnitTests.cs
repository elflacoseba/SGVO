using FluentAssertions;
using SGVO.Shared;

namespace SGVO.UnitTests.Shared;

/// <summary>
/// Tests unitarios para Unit type.
/// </summary>
public class UnitTests
{
    [Fact]
    public void Unit_Value_ShouldBeDefault()
    {
        Unit.Value.Should().Be(default(Unit));
    }

    [Fact]
    public void Unit_Instances_ShouldBeEqual()
    {
        Unit.Value.Should().Be(Unit.Value);
    }

    [Fact]
    public void Unit_Result_ShouldSucceed()
    {
        var result = Result<Unit>.Success(Unit.Value);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
    }
}

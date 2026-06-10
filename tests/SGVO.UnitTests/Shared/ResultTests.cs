using SGVO.Shared;

namespace SGVO.UnitTests.Shared;

/// <summary>
/// Tests unitarios para el patrón Result.
/// </summary>
public class ResultTests
{
    [Fact]
    public void Result_Success_Should_Be_Success()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Result_Failure_Should_Be_Failure()
    {
        var result = Result.Failure("Error de prueba", "TEST_ERROR");

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Error de prueba", result.Error);
        Assert.Equal("TEST_ERROR", result.ErrorCode);
    }

    [Fact]
    public void ResultOfT_Success_Should_Contain_Value()
    {
        var result = Result<string>.Success("valor");

        Assert.True(result.IsSuccess);
        Assert.Equal("valor", result.Value);
    }

    [Fact]
    public void ResultOfT_Failure_Should_Not_Contain_Value()
    {
        var result = Result<string>.Failure("Error");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ResultOfT_Map_Should_Transform_Value()
    {
        var result = Result<int>.Success(5)
            .Map(x => x * 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public void ResultOfT_Map_Should_Propagate_Error()
    {
        var result = Result<int>.Failure("Error original")
            .Map(x => x * 2);

        Assert.False(result.IsSuccess);
        Assert.Equal("Error original", result.Error);
    }
}

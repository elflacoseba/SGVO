using Microsoft.AspNetCore.Mvc;
using SGVO.Shared;

namespace SGVO.Api.Extensions;

/// <summary>
/// Métodos de extensión para convertir <see cref="Result"/> y <see cref="Result{T}"/> en <see cref="IActionResult"/>.
/// Mapea <see cref="Result.ErrorCode"/> a códigos HTTP estándar.
/// </summary>
public static class ApiResultExtensions
{
    /// <summary>
    /// Convierte un <see cref="Result"/> fallido en una respuesta HTTP con el código de estado apropiado.
    /// Si el resultado es exitoso, lanza <see cref="InvalidOperationException"/>.
    /// </summary>
    public static IActionResult ToErrorActionResult(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("ToErrorActionResult debe usarse solo cuando IsFailure es true.");

        return result.ErrorCode?.ToUpperInvariant() switch
        {
            "NOT_FOUND" => new NotFoundObjectResult(new { error = result.Error }),
            "VALIDATION" => new BadRequestObjectResult(new { error = result.Error }),
            "CONFLICT" => new ConflictObjectResult(new { error = result.Error }),
            "UNAUTHORIZED" => new UnauthorizedObjectResult(new { error = result.Error }),
            "FORBIDDEN" => new ObjectResult(new { error = result.Error }) { StatusCode = StatusCodes.Status403Forbidden },
            _ => new ObjectResult(new { error = result.Error }) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }

    /// <summary>
    /// Convierte un <see cref="Result{T}"/> fallido en una respuesta HTTP con el código de estado apropiado.
    /// Si el resultado es exitoso, lanza <see cref="InvalidOperationException"/>.
    /// </summary>
    public static IActionResult ToErrorActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("ToErrorActionResult debe usarse solo cuando IsFailure es true.");

        return result.ErrorCode?.ToUpperInvariant() switch
        {
            "NOT_FOUND" => new NotFoundObjectResult(new { error = result.Error }),
            "VALIDATION" => new BadRequestObjectResult(new { error = result.Error }),
            "CONFLICT" => new ConflictObjectResult(new { error = result.Error }),
            "UNAUTHORIZED" => new UnauthorizedObjectResult(new { error = result.Error }),
            "FORBIDDEN" => new ObjectResult(new { error = result.Error }) { StatusCode = StatusCodes.Status403Forbidden },
            _ => new ObjectResult(new { error = result.Error }) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}

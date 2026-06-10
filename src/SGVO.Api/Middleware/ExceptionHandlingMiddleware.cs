using Microsoft.AspNetCore.Mvc;

namespace SGVO.Api.Middleware;

/// <summary>
/// Middleware de manejo global de excepciones.
/// Captura excepciones no controladas, registra el error y devuelve
/// una respuesta ProblemDetails estandarizada.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor",
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}

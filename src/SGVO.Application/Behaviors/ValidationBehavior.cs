using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Behaviors;

/// <summary>
/// Pipeline de validación manual para comandos y consultas.
/// Como no usamos MediatR, este wrapper ejecuta FluentValidation antes de
/// delegar al manejador correspondiente.
/// </summary>
public static class ValidationBehavior
{
    /// <summary>
    /// Ejecuta la validación de una solicitud y retorna un Result<TResponse>
    /// con el error de validación si falla, o delega al manejador si pasa.
    /// </summary>
    public static async Task<Result<TResponse>> HandleAsync<TRequest, TResponse>(
        TRequest request,
        IValidator<TRequest> validator,
        Func<TRequest, CancellationToken, Task<Result<TResponse>>> handler,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First().ErrorMessage;
            return Result<TResponse>.Failure(error, "VALIDATION");
        }

        return await handler(request, cancellationToken);
    }
}

using FluentValidation;

namespace SGVO.Application.Validators;

/// <summary>
/// Validador base para el proyecto.
/// Proporciona reglas comunes reutilizables.
/// </summary>
public abstract class BaseValidator<T> : AbstractValidator<T>
    where T : class
{
    protected BaseValidator()
    {
        // Reglas globales o comunes pueden agregarse aquí.
    }
}

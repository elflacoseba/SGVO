namespace SGVO.Domain.Interfaces;

/// <summary>
/// Proveedor de fecha/hora para desacoplar el código del reloj del sistema.
/// Facilita el testing al permitir la inyección de un reloj controlado.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Obtiene la fecha/hora actual en UTC.
    /// </summary>
    DateTime UtcNow { get; }
}

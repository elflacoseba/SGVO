namespace SGVO.Infrastructure.Services;

using SGVO.Domain.Interfaces;

/// <summary>
/// Implementación del proveedor de fecha/hora que delega al reloj del sistema.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

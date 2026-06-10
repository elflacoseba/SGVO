namespace SGVO.Domain.Events;

/// <summary>
/// Contrato para eventos de dominio.
/// Los eventos de dominio permiten notificar cambios de estado dentro del dominio
/// de forma desacoplada.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único del evento.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Fecha/hora en que ocurrió el evento.
    /// </summary>
    DateTime OccurredOn { get; }
}

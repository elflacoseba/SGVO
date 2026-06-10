namespace SGVO.Domain.Events;

/// <summary>
/// Clase base para eventos de dominio.
/// Proporciona implementación común de IDomainEvent.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}

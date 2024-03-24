using CSharpFunctionalExtensions;

namespace Primitives;

public abstract class Aggregate: Entity<Guid>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
        
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
    
public interface IAggregateRoot
{
        
}
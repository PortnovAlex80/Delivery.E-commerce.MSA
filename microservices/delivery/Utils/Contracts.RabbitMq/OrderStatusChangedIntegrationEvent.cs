namespace Contracts.RabbitMq;

public record OrderStatusChangedIntegrationEvent()
{
    ///<summary>
    /// Идентификатор заказа
    /// </summary>
    public Guid OrderId { get; init; }
    
    ///<summary>
    /// Статус заказа
    /// </summary>
    public Status Status { get; init; }
}

/// <summary>
/// Период доставки
/// </summary>
public enum Status
{
    None,
    Created,
    Assigned,
    Completed
}
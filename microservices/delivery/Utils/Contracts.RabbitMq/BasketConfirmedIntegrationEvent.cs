namespace Contracts.RabbitMq;

public record BasketConfirmedIntegrationEvent()
{
    /// <summary>
    /// Идентификатор корзины
    /// </summary>
    public Guid BasketId { get; init; }

    /// <summary>
    ///     Улица
    /// </summary>
    public string Street { get; init; }

    /// <summary>
    ///     Дом
    /// </summary>
    public int Weight { get; init; }
}
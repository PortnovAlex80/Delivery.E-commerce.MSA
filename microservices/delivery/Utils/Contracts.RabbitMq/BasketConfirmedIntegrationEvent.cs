namespace Contracts.RabbitMq;

public record BasketConfirmedIntegrationEvent()
{
    /// <summary>
    /// Идентификатор корзины
    /// </summary>
    public Guid BasketId { get; init; }

    /// <summary>
    /// Идентификатор покупателя
    /// </summary>
    public Guid BuyerId { get; init; }
    
    /// <summary>
    ///     Товарные позиции
    /// </summary>
    public List<Item> Items { get; set; } = new();

    ///<summary>
    ///     Страна
    /// </summary>
    public string Country { get; init; }

    /// <summary>
    ///     Город
    /// </summary>
    public string City { get; init; }

    /// <summary>
    ///     Улица
    /// </summary>
    public string Street { get; init; }

    /// <summary>
    ///     Дом
    /// </summary>
    public string House { get; init; }

    /// <summary>
    ///     Квартира
    /// </summary>
    public string Apartment { get; init; }

    /// <summary>
    ///     Период доставки
    /// </summary>
    public TimeSlot TimeSlot { get; init; }
}

/// <summary>
/// Период доставки
/// </summary>
public enum TimeSlot
{
    None,
    Morning,
    Midday,
    Evening,
    Night
}

public record Item
{

    /// <summary>
    /// Идентификтор товара
    /// </summary>
    public virtual Guid GoodId { get; init; }

    /// <summary>
    /// Количество
    /// </summary>
    public virtual int Quantity { get; init; }
}
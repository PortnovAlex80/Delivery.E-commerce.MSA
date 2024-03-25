using Contracts.RabbitMq;
using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MassTransit;

namespace DeliveryApp.Infrastructure.Adapters.RabbitMq;

public sealed class RabbitMqBusProducer : IBusProducer
{
    readonly IBus _bus;
    
    public RabbitMqBusProducer(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishOrderCreatedDomainEvent(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent()
        {
            OrderId = notification.Order.Id,
            Status = (Status)notification.Order.Status.Id
        };
        await _bus.Publish(orderStatusChangedIntegrationEvent, cancellationToken);
    }

    public async Task PublishOrderAssignedDomainEvent(OrderAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent()
        {
            OrderId = notification.Order.Id,
            Status = (Status)notification.Order.Status.Id
        };
        await _bus.Publish(orderStatusChangedIntegrationEvent, cancellationToken);
    }

    public async Task PublishOrderCompletedDomainEvent(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent()
        {
            OrderId = notification.Order.Id,
            Status = (Status)notification.Order.Status.Id
        };
        await _bus.Publish(orderStatusChangedIntegrationEvent, cancellationToken);
    }
}
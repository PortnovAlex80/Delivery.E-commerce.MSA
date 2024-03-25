using Contracts.RabbitMq;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using MassTransit;
using MediatR;

namespace DeliveryApp.Ui.Adapters.RabbitMq
{
    public class BasketConfirmedConsumer : IConsumer<BasketConfirmedIntegrationEvent>
    {
        private readonly IMediator _mediator;

        public BasketConfirmedConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<BasketConfirmedIntegrationEvent> context)
        {
            
        }
    }
}
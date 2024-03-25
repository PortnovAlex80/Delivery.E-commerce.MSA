using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetOrder
{
    public class Query : IRequest<Response>
    {
        public Guid OrderId { get; private set; }

        public Query(Guid orderId)
        {
            if (orderId == Guid.Empty) throw new ArgumentException(nameof(orderId));
            OrderId = orderId;
        }
    }
}
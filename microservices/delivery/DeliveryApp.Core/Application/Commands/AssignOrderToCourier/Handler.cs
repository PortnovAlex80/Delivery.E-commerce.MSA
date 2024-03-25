using DeliveryApp.Core.DomainServices;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            // Восстанавливаем аггрегаты
            var order = _orderRepository.GetAllActive().FirstOrDefault();
            if (order == null) return false;
            var couriers = _courierRepository.GetAllActive().ToList();
            if (!couriers.Any()) return false;

            // Распределяем заказы на курьеров
            var dispatchResult = DispatchService.Dispatch(order, couriers);
            if (dispatchResult.IsFailure) return false;
            var courier = dispatchResult.Value;

            _courierRepository.Update(courier);
            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

             Console.WriteLine("Use case Assigned to order" + courier.Id + " " + order.Id);

            //await _orderRepository.UnitOfWork.CommitTransactionAsync(transaction);

            return true;
        }
    }
}
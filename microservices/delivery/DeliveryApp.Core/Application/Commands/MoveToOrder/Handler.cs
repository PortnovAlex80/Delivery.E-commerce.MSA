﻿using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveToOrder
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
            var assignedOrders = _orderRepository.GetAllAssigned();
            if (!assignedOrders.Any())
                return false;

            // Изменяем аггрегаты
            foreach (var order in assignedOrders)
            {
                if (order.CourierId == null)
                    return false;
                
                var courier = await _courierRepository.GetAsync((Guid) order.CourierId);
                if (courier == null) return false;

                var courierMoveResult = courier.Move(order.Location);
                if (courierMoveResult.IsFailure) return false;

                // Если дошли - завершаем заказ, освобождаем курьера
                if (order.Location == courier.Location)
                {
                    order.Complete();
                    courier.CompleteOrder();
                }

                // Сохраняем аггрегат
                _courierRepository.Update(courier);
                _orderRepository.Update(order);
            }
            await _courierRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return true;
        }
    }
}
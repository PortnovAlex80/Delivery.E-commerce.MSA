﻿using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CompleteOrder
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            //Восстанавливаем аггрегат
            var order = await _orderRepository.GetAsync(message.OrderId);
            
            //Изменяем аггрегат
            var orderCompleteResult = order.Complete();
            if (orderCompleteResult.IsFailure) return false;

            //Сохраняем аггрегат
            _orderRepository.Update(order);
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
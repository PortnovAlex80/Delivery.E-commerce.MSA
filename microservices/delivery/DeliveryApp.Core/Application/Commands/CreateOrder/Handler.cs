﻿using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGeoClient _geoClient;

        public Handler(IOrderRepository orderRepository, IGeoClient geoClient)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
             _geoClient = geoClient ?? throw new ArgumentNullException(nameof(geoClient));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            //Получаем геопозицию из Geo 
            var location = await _geoClient.GetGeolocationAsync(message.Address,cancellationToken);
            Console.WriteLine("Location - " + location);
            
            //Создаем вес
            var weightCreateResult = Weight.Create(message.Weight);
            if (weightCreateResult.IsFailure) return false;
            var weight = weightCreateResult.Value;
            
            //Создаем заказ
            var orderCreateResult = Order.Create(message.BasketId,location,weight);
            if (orderCreateResult.IsFailure) return false;
            var order = orderCreateResult.Value;
            
            //Сохраняем аггрегат
            _orderRepository.Add(order);
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
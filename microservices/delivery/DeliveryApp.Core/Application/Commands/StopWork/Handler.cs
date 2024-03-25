﻿using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.StopWork
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            //Восстанавливаем аггрегат
            var courier = await _courierRepository.GetAsync(message.CourierId);
            if (courier == null) return false;
            
            //Изменяем аггрегат
            var courierStartWorkResult = courier.StopWork();
            if (courierStartWorkResult.IsFailure) return false;

            //Сохраняем аггрегат
            _courierRepository.Update(courier);
            return await _courierRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
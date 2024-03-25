﻿using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CompleteOrder
{
    /// <summary>
    /// Отменить заказ
    /// </summary>
    public class Command : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid OrderId { get; private set; }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        public Command(Guid orderId)
        {
            if (orderId == Guid.Empty) throw new ArgumentException(nameof(orderId));
            OrderId = orderId;
        }
    }
}
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    /// <summary>
    ///     Заказ
    /// </summary>
    public class Order : Aggregate
    {
        public static class Errors
        {
            public static Error CantCompletedNotAssignedOrder()
            {
                return new($"{nameof(Order).ToLowerInvariant()}.cant.completed.not.sssigned.order",
                    "Нельза завершить заказ, который не был назначен");
            }
        }

        /// <summary>
        /// Идентификатор исполнителя (курьера)
        /// </summary>
        public virtual Guid? CourierId { get; protected set; }
        
        /// <summary>
        /// Геопозиция
        /// </summary>
        public virtual Location Location { get; protected set; }
        
        /// <summary>
        /// Вес
        /// </summary>
        public virtual Weight Weight { get; protected set;}
        
        /// <summary>
        /// Статус
        /// </summary>
        public virtual OrderStatus Status { get; protected set; }
      
        /// <summary>
        /// Ctr
        /// </summary>
        protected Order() { }
        
        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="location">Геопозиция</param>
        /// <param name="weight">Вес</param>
        protected Order(Guid orderId, Location location, Weight weight)
        {
            Id = orderId;
            Location = location;
            Weight = weight;
            Status = OrderStatus.Created;
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="location">Геопозиция</param>
        /// <param name="weight">Вес</param>
        /// <returns>Результат</returns>
        public static Result<Order, Error> Create(Guid orderId, Location location, Weight weight)
        {
            if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
            if (weight == null) return GeneralErrors.ValueIsRequired(nameof(weight));
            return new Order(orderId, location, weight);
        }
        
        /// <summary>
        /// Назначить заказ на курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Результат</returns>
        public Result<object, Error> AssignToCourier(Courier courier)
        {
            if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));
            if (courier.Status == CourierStatus.Busy) return GeneralErrors.ValueIsRequired(nameof(courier)); // TODO
            
            CourierId = courier.Id;
            Status = OrderStatus.Assigned;
            courier.InWork();
            
            return new object();
        }
        
        /// <summary>
        /// Завершить выполнение заказа
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> Complete()
        {
            if (Status != OrderStatus.Assigned) return Errors.CantCompletedNotAssignedOrder();
            
            Status = OrderStatus.Completed;
            return new object();
        }
    }
}
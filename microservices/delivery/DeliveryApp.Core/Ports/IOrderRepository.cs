using DeliveryApp.Core.Domain.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Repository для Aggregate Order
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Заказ</returns>
        Order Add(Order order);

        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="order">Заказ</param>
        void Update(Order order);

        /// <summary>
        /// Получить
        /// </summary>
        /// <param name="orderId">Идентификатор</param>
        /// <returns>Заказ</returns>
        Task<Order> GetAsync(Guid orderId);

        /// <summary>
        /// Получить все нераспределенные заказы
        /// </summary>
        /// <returns>Заказы</returns>
        IEnumerable<Order> GetAllActive();
        
        /// <summary>
        /// Получить все распределенные заказы
        /// </summary>
        /// <returns>Заказы</returns>
        IEnumerable<Order> GetAllAssigned();
        
        /// <summary>
        /// Получить заказ по Id курьера
        /// </summary>
        /// <returns>Заказ</returns>
        Task<Order> GetByCourierId(Guid courierId);
    }
}
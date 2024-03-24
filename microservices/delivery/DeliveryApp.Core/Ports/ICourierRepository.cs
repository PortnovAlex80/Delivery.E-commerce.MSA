using DeliveryApp.Core.Domain.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Repository для Aggregate Courier
    /// </summary>
    public interface ICourierRepository : IRepository<Courier>
    {
        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Курьер</returns>
        Courier Add(Courier courier);

        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="courier">Курьер</param>
        void Update(Courier courier);

        /// <summary>
        /// Получить
        /// </summary>
        /// <param name="courierId">Идентификатор</param>
        /// <returns>Курьер</returns>
        Task<Courier> GetAsync(Guid courierId);

        /// <summary>
        /// Получить всех свободных курьеров
        /// </summary>
        /// <returns>Курьеры</returns>
        IEnumerable<Courier> GetAllActive();
        
        /// <summary>
        /// Получить всех занятых курьеров
        /// </summary>
        /// <returns>Курьеры</returns>
        IEnumerable<Courier> GetAllBusy();
    }
}
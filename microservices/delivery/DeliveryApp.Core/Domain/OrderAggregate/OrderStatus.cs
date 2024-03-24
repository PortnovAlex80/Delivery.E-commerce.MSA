using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    /// <summary>
    ///     Статус
    /// </summary>
    public class OrderStatus : Entity<int>
    {
        public static readonly OrderStatus Created = new(1, nameof(Created).ToLowerInvariant());
        public static readonly OrderStatus Assigned = new(2, nameof(Assigned).ToLowerInvariant());
        public static readonly OrderStatus Completed = new(3, nameof(Completed).ToLowerInvariant());
    
        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new($"{nameof(OrderStatus).ToLowerInvariant()}.is.wrong", 
                    $"Не верное значение. Допустимые значения: {nameof(OrderStatus).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
            }
        }
        
        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctr
        /// </summary>
        protected OrderStatus()
        {}
    
        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        protected OrderStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<OrderStatus> List()
        {
            yield return Created;
            yield return Assigned;
            yield return Completed;
        }

        /// <summary>
        /// Получить статус по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Статус</returns>
        public static Result<OrderStatus, Error> FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }

        /// <summary>
        /// Получить статус по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Статус</returns>
        public static Result<OrderStatus, Error> From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }
    }
}

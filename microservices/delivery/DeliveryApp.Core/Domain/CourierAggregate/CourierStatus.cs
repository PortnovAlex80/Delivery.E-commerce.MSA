using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    /// <summary>
    ///     Статус
    /// </summary>
    public class CourierStatus : Entity<int>
    {
        public static readonly CourierStatus NotAvailable = new(1, nameof(NotAvailable).ToLowerInvariant());
        public static readonly CourierStatus Ready = new(2, nameof(Ready).ToLowerInvariant());
        public static readonly CourierStatus Busy = new(3, nameof(Busy).ToLowerInvariant());
    
        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new($"{nameof(CourierStatus).ToLowerInvariant()}.is.wrong", 
                    $"Не верное значение. Допустимые значения: {nameof(CourierStatus).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
            }
        }
        
        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctr
        /// </summary>
        protected CourierStatus()
        {}
    
        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        protected CourierStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<CourierStatus> List()
        {
            yield return NotAvailable;
            yield return Ready;
            yield return Busy;
        }

        /// <summary>
        /// Получить статус по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Статус</returns>
        public static Result<CourierStatus, Error> FromName(string name)
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
        public static Result<CourierStatus, Error> From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }
    }
}

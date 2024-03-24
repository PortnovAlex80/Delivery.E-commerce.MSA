using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    /// <summary>
    ///     Статус
    /// </summary>
    public class Transport : Entity<int>
    {
        public static readonly Transport Pedestrian = new(1, nameof(Pedestrian).ToLowerInvariant(), 1, Weight.Create(1).Value);
        public static readonly Transport Bicycle = new(2, nameof(Bicycle).ToLowerInvariant(),2, Weight.Create(4).Value);
        public static readonly Transport Scooter = new(3, nameof(Scooter).ToLowerInvariant(),3, Weight.Create(6).Value);
        public static readonly Transport Car = new(4, nameof(Car).ToLowerInvariant(),4, Weight.Create(8).Value);
    
        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new($"{nameof(Transport).ToLowerInvariant()}.is.wrong", 
                    $"Не верное значение. Допустимые значения: {nameof(Transport).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
            }
        }
        
        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; }
    
        /// <summary>
        ///     Скорость
        /// </summary>
        public int Speed { get; }
    
        /// <summary>
        ///     Грузоподъемность
        /// </summary>
        public Weight Capacity { get; }
        
        /// <summary>
        /// Ctr
        /// </summary>
        protected Transport()
        {}
    
        /// <summary>
        ///     Ctr
        /// </summary>
        protected Transport(int id, string name, int speed, Weight capacity)
        {
            Id = id;
            Name = name;
            Speed = speed;
            Capacity = capacity;
        }
        
        /// <summary>
        ///     Ехать в вверх
        /// </summary>
        public Result<Location, Error> GoUp(Location location, out int leftoverSteps)
        {
            leftoverSteps = 0;
            var y = location.Y + Speed;
            if (y > Location.MaxLocation.Y)
            {
                leftoverSteps = y - Location.MaxLocation.Y;
                y = Location.MaxLocation.Y;
            }
            var result = Location.Create(location.X,y);
            if (result.IsFailure) return result.Error;
            return result.Value;
        }
        
        /// <summary>
        ///     Ехать в вниз
        /// </summary>
        public Result<Location, Error> GoDown(Location location, out int leftoverSteps)
        {
            leftoverSteps = 0;
            var y = location.Y - Speed;
            if (y < Location.MinLocation.Y)
            {
                leftoverSteps = y + Location.MinLocation.Y;
                y = Location.MinLocation.Y;
            }
            var result = Location.Create(location.X,y);
            if (result.IsFailure) return result.Error;
            return result.Value;
        }
        
        /// <summary>
        ///     Ехать в лево
        /// </summary>
        public Result<Location, Error> GoLeft(Location location, out int leftoverSteps)
        {
            leftoverSteps = 0;
            var x = location.X - Speed;
            if (x < Location.MinLocation.X)
            {
                leftoverSteps = x + Location.MinLocation.X;
                x = Location.MinLocation.X;
            }
            var result = Location.Create(x,location.Y);
            if (result.IsFailure) return result.Error;
            return result.Value;
        }
        
        /// <summary>
        ///     Ехать в право
        /// </summary>
        public Result<Location, Error> GoRight(Location location, out int leftoverSteps)
        {
            leftoverSteps = 0;
            var x = location.X + Speed;
            if (x > Location.MaxLocation.X)
            {
                leftoverSteps = x - Location.MaxLocation.X;
                x = Location.MaxLocation.X;
            }
            var result = Location.Create(x,location.Y);
            if (result.IsFailure) return result.Error;
            return result.Value;
        }
        
        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<Transport> List()
        {
            yield return Pedestrian;
            yield return Bicycle;
            yield return Scooter;
            yield return Car;
        }

        /// <summary>
        /// Получить транспорт по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Транспорт</returns>
        public static Result<Transport, Error> FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }

        /// <summary>
        /// Получить транспорт по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Транспорт</returns>
        public static Result<Transport, Error> From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }
        
        /// <summary>
        /// Может ли данный транспорт перевезти данный вес
        /// </summary>
        /// <param name="weight">Вес</param>
        /// <returns>Результат</returns>
        public Result<bool, Error> CanAllocate(Weight weight)
        {
            if (weight > Capacity)
            {
                return false;
            }
            return true;
        }
    }
}

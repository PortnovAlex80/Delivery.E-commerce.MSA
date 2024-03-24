using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    /// <summary>
    ///     Курьер
    /// </summary>
    public class Courier : Aggregate
    {
        public static class Errors
        {
            public static Error TryStopWorkingWithIncompleteDelivery()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.stop.working.with.incomplete.delivery", "Нельзя прекратить работу, если есть незавершенная доставка");
            }
            
            public static Error TryStartWorkingWhenAlreadyStarted()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.start.working.when.already.started", "Нельзя начать работу, если ее уже начали ранее");
            }
            
            public static Error TryAssignOrderWhenNotAvailable()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.assign.order.when.not.available", "Нельзя взять заказ в работу, если курьер не начал рабочий день");
            }
        }
        
        /// <summary>
        /// Имя
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Вид транспорта
        /// </summary>
        public virtual Transport Transport { get; protected set; }
        
        /// <summary>
        /// Геопозиция (X,Y)
        /// </summary>
        public virtual Location Location { get; protected set; }
        
        /// <summary>
        /// Статус курьера
        /// </summary>
        public CourierStatus Status { get; protected set; }
        
        /// <summary>
        /// Ctr
        /// </summary>
        protected Courier() {}
        
        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="transport">Транспорт</param>
        protected Courier(string name, Transport transport):this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Transport = transport;
            Location = Location.MinLocation;
            Status = CourierStatus.NotAvailable;
        }

        /// <summary>
        ///  Factory Method
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="transport">Транспорт</param>
        /// <returns>Результат</returns>
        public static Result<Courier, Error> Create(string name, Transport transport)
        {
            if (string.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (transport == null) return GeneralErrors.ValueIsRequired(nameof(transport));
            
            return new Courier(name,transport);
        }
        
        /// <summary>
        /// Изменить местоположение
        /// </summary>
        /// <param name="targetLocation">Геопозиция</param>
        /// <returns>Результат</returns>
        public Result<object, Error> Move(Location targetLocation)
        {
            if (targetLocation == null) return GeneralErrors.ValueIsRequired(nameof(targetLocation));
            if (targetLocation == Location) return new object();

            for (;;)
            {
                int leftoverSteps;
                if (Location.X < targetLocation.X)
                {
                    // Если курьер правее цели, то двигаемся налево
                    var transportGoRightResult = Transport.GoRight(Location,out leftoverSteps);
                    if (transportGoRightResult.IsFailure) return transportGoRightResult.Error;

                    Location = transportGoRightResult.Value;
                    if (leftoverSteps==0)
                    {
                        break;
                    }
                }
                else if (Location.X > targetLocation.X)
                {
                    // Если курьер правее цели, то двигаемся налево
                    var transportGoRightResult = Transport.GoLeft(Location,out leftoverSteps);
                    if (transportGoRightResult.IsFailure) return transportGoRightResult.Error;
                    
                    Location = transportGoRightResult.Value;
                    if (leftoverSteps==0)
                    {
                        break;
                    }
                }
                else if (Location.Y < targetLocation.Y)
                {
                    // Если курьер ниже цели, то двигаемся вверх
                    var transportGoRightResult = Transport.GoUp(Location,out leftoverSteps);
                    if (transportGoRightResult.IsFailure) return transportGoRightResult.Error;
                    
                    Location = transportGoRightResult.Value;
                    if (leftoverSteps==0)
                    {
                        break;
                    }
                }
                else if (Location.Y > targetLocation.Y)
                {
                    // Если курьер выше цели, то двигаемся вниз
                    var transportGoRightResult = Transport.GoDown(Location,out leftoverSteps);
                    if (transportGoRightResult.IsFailure) return transportGoRightResult.Error;
                    
                    Location = transportGoRightResult.Value;
                    if (leftoverSteps==0)
                    {
                        break;
                    }
                }
            }
            return new object();
        }
        
        /// <summary>
        /// Начать работать
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> StartWork()
        {
            if (Status == CourierStatus.Busy) return Errors.TryStartWorkingWhenAlreadyStarted();
            Status = CourierStatus.Ready;
            return new object();
        }
        
        /// <summary>
        /// Взять работу
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> InWork()
        {
            if (Status == CourierStatus.NotAvailable) return Errors.TryAssignOrderWhenNotAvailable();
            Status = CourierStatus.Busy;
            return new object();
        }
        
        /// <summary>
        /// Завершить работу
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> CompleteOrder()
        {
            Status = CourierStatus.Ready;
            return new object();
        }
        
        /// <summary>
        /// Закончить работать
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> StopWork()
        {
            if (Status == CourierStatus.Busy) return Errors.TryStopWorkingWithIncompleteDelivery();
            Status = CourierStatus.NotAvailable;
            return new object();
        }
        
        /// <summary>
        /// Рассчитать время до точки
        /// </summary>
        /// <param name="location">Конечное местоположение</param>
        /// <returns>Результат</returns>
        public Result<double, Error> CalculateTimeToLocation(Location location)
        {
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
            
            var distance = Location.DistanceTo(location).Value;
            var time = (double) distance / Transport.Speed;
            return time;
        }
    }
}
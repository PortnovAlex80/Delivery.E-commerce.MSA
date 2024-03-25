using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.DomainServices;

public class DispatchService
{
    /// <summary>
    /// Распределить заказ на курьера
    /// </summary>
    /// <returns>Результат</returns>
    public static Result<Courier, Error> Dispatch(Order order,List<Courier> couriers)
    {
        if (order==null) return GeneralErrors.ValueIsRequired(nameof(order));
        if (!couriers.Any()) return GeneralErrors.InvalidLength(nameof(couriers));
        
        var scores = new List<Score>();
        foreach (var courier in couriers)
        {
            var time = courier.CalculateTimeToLocation(order.Location).Value;
            scores.Add(new Score() { Courier = courier, TimeToLocation = time });
        }
        var bestScore = scores.MinBy(c => c.TimeToLocation);
        var assignToCourierResult = order.AssignToCourier(bestScore.Courier);
        if (assignToCourierResult.IsFailure) return assignToCourierResult.Error;

        return bestScore.Courier;
    }
    
    public class Score
    {
        public Courier Courier { get; set; }
        
        public double TimeToLocation { get; set; }
    }
}
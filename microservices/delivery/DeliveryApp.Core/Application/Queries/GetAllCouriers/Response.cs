﻿﻿namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;

public class Response
{
    public List<Courier> Couriers { get; set; } = new List<Courier>();

    public Response(List<Courier> couriers)
    {
        Couriers.AddRange(couriers);
    }
}

public class Courier
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Геопозиция (X,Y)
    /// </summary>
    public Location Location { get; set; }
}

public class Location
{
    /// <summary>
    /// Горизонталь
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Вертикаль
    /// </summary>
    public int Y { get; set; }
}
namespace DeliveryApp.Core.Application.UseCases.Queries.GetOrder
{
    public class Response
    {
        public Order Order { get; set; }

        public Response(Order order)
        {
            Order = order;
        }
    }


    public class Order
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Курьер
        /// </summary>
        public Courier Courier { get; set; }
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
        
        /// <summary>
        /// Транспорт
        /// </summary>
        public string Transport { get; set; }
        
        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }
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
}
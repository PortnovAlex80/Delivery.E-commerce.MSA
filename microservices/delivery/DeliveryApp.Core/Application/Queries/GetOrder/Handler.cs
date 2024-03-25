using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetOrder
{
    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly string _connectionString;
 
        public Handler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Response> Handle(Query message, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT o.*,os.name as order_status,c.*,cs.name as courier_status ,t.name as transport  FROM public.orders as o 
                        INNER JOIN public.couriers as c on o.courier_id=c.id
                        INNER JOIN public.transports as t on c.transport_id=t.id
                        INNER JOIN public.order_statuses as os on o.status_id=os.id  
                        INNER JOIN public.courier_statuses as cs on c.status_id=cs.id                                                                
                        WHERE o.id=@id;"
                , new { id = message.OrderId });

            if (result.AsList().Count == 0)
                return null;

            return new Response(MapToOrder(result));
        }
    
        private Order MapToOrder(dynamic result)
        {
            var courierLocation = new Location{X = result[0].location_x, Y = result[0].location_y};
            var courier = new Courier{Id = result[0].courier_id, Name = result[0].name, Location = courierLocation, Transport = result[0].transport, Status = result[0].courier_status};
            var order = new Order {Id = result[0].id, Courier = courier, Status = result[0].order_status};
            return order;
        }
    }
}
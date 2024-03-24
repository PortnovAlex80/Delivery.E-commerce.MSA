using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IUnitOfWork UnitOfWork => _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Order Add(Order order)
        {
            _dbContext.Attach(order.Status);
            return _dbContext.Orders.Add(order).Entity;
        }
        
        public void Update(Order order)
        {
            _dbContext.Attach(order.Status);
            _dbContext.Entry(order).State = EntityState.Modified;
        }

        public async Task<Order> GetAsync(Guid orderId)
        {
            var order = await _dbContext
                .Orders
                .Include(x => x.Status)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            return order;
        }

        public IEnumerable<Order> GetAllActive()
        {
            var orders = _dbContext
                .Orders
                .Include(x => x.Status)
                .Where(o => o.Status == OrderStatus.Created);
            return orders;
        }
        
        public IEnumerable<Order> GetAllAssigned()
        {
            var orders = _dbContext
                .Orders
                .Include(x => x.Status)
                .Where(o => o.Status == OrderStatus.Assigned);
            return orders;
        }

        public async Task<Order> GetByCourierId(Guid courierId)
        {
            var orders = await _dbContext
                .Orders
                .Include(x => x.Status)    
                .FirstOrDefaultAsync(o => o.CourierId == courierId);
            return orders;
        }
    }
}
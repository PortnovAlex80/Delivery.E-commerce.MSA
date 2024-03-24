using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IUnitOfWork UnitOfWork => _dbContext;

        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Courier Add(Courier courier)
        {
            _dbContext.Attach(courier.Status);
            _dbContext.Attach(courier.Transport);
            return _dbContext.Couriers.Add(courier).Entity;
        }
        
        public void Update(Courier courier)
        {
            _dbContext.Attach(courier.Status);
            _dbContext.Attach(courier.Transport);
            _dbContext.Entry(courier).State = EntityState.Modified;
        }

        public async Task<Courier> GetAsync(Guid courierId)
        {
            var courier = await _dbContext
                .Couriers
                .Include(x => x.Status)
                .Include(x => x.Transport)
                .FirstOrDefaultAsync(o => o.Id == courierId);
            return courier;
        }

        public IEnumerable<Courier> GetAllActive()
        {
            var couriers = _dbContext
                .Couriers
                .Include(x => x.Status)
                .Include(x => x.Transport)
                .Where(o => o.Status == CourierStatus.Ready);
            return couriers;
        }
        
        public IEnumerable<Courier> GetAllBusy()
        {
            var couriers = _dbContext
                .Couriers
                .Include(x => x.Status)
                .Include(x => x.Transport)
                .Where(o => o.Status == CourierStatus.Busy);
            return couriers;
        }
    }
}
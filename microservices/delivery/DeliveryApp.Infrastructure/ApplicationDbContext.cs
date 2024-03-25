﻿using System.Data;
using BasketApp.Infrastructure.EntityConfigurations.Outbox;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Infrastructure.Entities;
using DeliveryApp.Infrastructure.EntityConfigurations.CourierAggregate;
using DeliveryApp.Infrastructure.EntityConfigurations.OrderAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Primitives;

namespace DeliveryApp.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Courier> Couriers { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        
        private readonly IMediator _mediator;
        
        private IDbContextTransaction _currentTransaction;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order Aggregate
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            
            // Order statuses
            modelBuilder.Entity<OrderStatus>(b =>
            {
                var allStatuses = OrderStatus.List();
                b.HasData(allStatuses.Select(c=>new { c.Id,c.Name }));
            });
            
            //Outbox
            modelBuilder.ApplyConfiguration(new OutboxEntityTypeConfiguration());

            // Courier Aggregate
            modelBuilder.ApplyConfiguration(new CourierEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourierStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransportEntityTypeConfiguration());
        
            // Courier statuses
            modelBuilder.Entity<CourierStatus>(b =>
            {
                var allStatuses = CourierStatus.List();
                b.HasData(allStatuses.Select(c=>new { c.Id,c.Name }));
            });
            
            // Courier transports
            modelBuilder.Entity<Transport>(b =>
            {
                var allTransports = Transport.List();
                b.HasData(allTransports.Select(c=>new { c.Id,c.Name,c.Speed,c.Capacity.Value }));
            });
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveDomainEventsInOutboxEventsAsync(this);
            await base.SaveChangesAsync(cancellationToken);
            return true;
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        async Task SaveDomainEventsInOutboxEventsAsync(ApplicationDbContext dbContext)
{
        // Достаем доменные события из Aggregate и преобразовываем их к OutboxMessage
        var outboxMessages = dbContext.ChangeTracker
            .Entries<Aggregate>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
                {
                    var domainEvents = aggregate.GetDomainEvents();
                    aggregate.ClearDomainEvents();
                    return domainEvents;
                }
            )
            .Select(domainEvent => new OutboxMessage()
            {
                Id = domainEvent.Id,
                OccuredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })  
            })
            .ToList();

        // Добавяляем OutboxMessage в dbContext, после выхода из метода они и сам Aggregate будут сохранены
        await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages);
}
    }
}
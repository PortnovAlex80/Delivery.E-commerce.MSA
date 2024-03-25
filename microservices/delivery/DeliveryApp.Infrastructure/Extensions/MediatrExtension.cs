using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Extensions;

static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, ApplicationDbContext ctx)
    {
        // Получаем доменные сущности типа Aggregate, в которых есть Domain Events
        var domainEntities = ctx.ChangeTracker
            .Entries<Aggregate>()
            .Where(x => x.Entity.DomainEvents.Any());

        // Копируем Domain Events в отдельную переменную
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // Очищаем Domain Events в доменной сущности
        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        // Отправляем все Domain Events в MediatR. Mediatr же их доставит в Handlers, где они и будут обработаны
        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}

using BasketApp.Core.Domain.BasketAggregate.DomainEvents;

namespace BasketApp.Core.Ports;

public interface IBusProducer
{
    Task Publish(BasketConfirmedDomainEvent notification, CancellationToken cancellationToken);
}
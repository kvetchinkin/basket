using BasketApp.Core.Domain.BasketAggregate.DomainEvents;
using BasketApp.Core.Ports;
using MediatR;

namespace BasketApp.Core.Application.DomainEventHandlers;

public sealed class BasketConfirmedDomainEventHandler : INotificationHandler<BasketConfirmedDomainEvent>
{
    readonly IBusProducer _busProducer;

    public BasketConfirmedDomainEventHandler(IBusProducer busProducer)
    {
        _busProducer = busProducer;
    }

    public async Task Handle(BasketConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _busProducer.Publish(notification, cancellationToken);
    }
}
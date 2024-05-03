using Primitives;

namespace BasketApp.Core.Domain.BasketAggregate.DomainEvents
{
    public sealed record BasketConfirmedDomainEvent(Guid BasketId, string Address, int Weight) : DomainEvent;
}


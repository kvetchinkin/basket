using MediatR;

namespace BasketApp.Core.Application.UseCases.Queries.GetBasket;

/// <summary>
/// Получить состав корзины
/// </summary>
public class Query : IRequest<Response>
{
    public Guid BasketId { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Query()
    {}
    
    public Query(Guid basketId)
    {
        if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
        BasketId = basketId;
    }
}
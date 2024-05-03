using MediatR;

namespace BasketApp.Core.Application.UseCases.Commands.Checkout;

/// <summary>
/// Оформить корзину
/// </summary>
public class Command : IRequest<bool>
{
    /// <summary>
    /// Идентификатор корзины
    /// </summary>
    public Guid BasketId { get; }
    
    /// <summary>
    /// Ctr
    /// </summary>
    private Command()
    {}

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param>
    public Command(Guid basketId)
    {
        if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
        BasketId = basketId;
    }
}
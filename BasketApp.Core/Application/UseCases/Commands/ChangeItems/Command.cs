using MediatR;

namespace BasketApp.Core.Application.UseCases.Commands.ChangeItems;

/// <summary>
/// Изменить состав корзины
/// </summary>
public class Command : IRequest<bool>
{
    /// <summary>
    /// Идентификатор покупателя
    /// </summary>
    public Guid BuyerId { get; }

    /// <summary>
    /// Идентификатор товара
    /// </summary>
    public Guid GoodId { get; }

    /// <summary>
    /// Количество позиций
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Command()
    {}
    
    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="buyerId">Идентификатор покупателя</param> 
    /// <param name="goodId">Идентификатор товара</param>
    /// <param name="quantity">Количество позиций</param>
    public Command(Guid buyerId, Guid goodId, int quantity)
    {
        if (buyerId == Guid.Empty) throw new ArgumentException(nameof(buyerId));
        if (goodId == Guid.Empty) throw new ArgumentException(nameof(goodId));
        if (quantity < 0) throw new ArgumentException(nameof(quantity));

        BuyerId = buyerId;
        GoodId = goodId;
        Quantity = quantity;
    }
}
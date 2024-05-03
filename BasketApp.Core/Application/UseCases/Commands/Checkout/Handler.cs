using BasketApp.Core.Ports;
using MediatR;
using Primitives;

namespace BasketApp.Core.Application.UseCases.Commands.Checkout;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepository;
    private readonly IDiscountClient _discountClient;

    /// <summary>
    /// Ctr
    /// </summary>
    private Handler()
    {}
    
    public Handler(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IDiscountClient discountClient)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _discountClient = discountClient ?? throw new ArgumentNullException(nameof(discountClient));
    }

    public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
    {
        //Восстанавливаем аггрегат
        var basket = await _basketRepository.GetAsync(message.BasketId);
        if (basket == null) return false;

        // Забираем скидку из сервиса Discount
        var discount = await _discountClient.GetDiscountAsync(basket,cancellationToken);

        // Оформляем заказ со скидкой
        var basketCheckoutResult = basket.Checkout(discount);
        if (basketCheckoutResult.IsFailure) return false;

        //Сохраняем аггрегат
        _basketRepository.Update(basket);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
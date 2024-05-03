using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Ports;
using MediatR;
using Primitives;

namespace BasketApp.Core.Application.UseCases.Commands.ChangeItems;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepository;
    private readonly IGoodRepository _goodRepository;

    /// <summary>
    /// Ctr
    /// </summary>
    private Handler()
    {}
    
    /// <summary>
    /// Ctr
    /// </summary>
    public Handler(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IGoodRepository goodRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _goodRepository = goodRepository ?? throw new ArgumentNullException(nameof(goodRepository));
    }

    public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
    {
        //Восстанавливаем аггрегат
        var good = await _goodRepository.GetAsync(message.GoodId);
        if (good == null)
        {
            return false;
        }

        var basket = await _basketRepository.GetAsync(message.BuyerId);

        if (basket == null)
        {
            var result = Basket.Create(message.BuyerId);
            if (result.IsFailure)
            {
                return false;
            }

            basket = result.Value;

            //Изменяем аггрегат
            basket.Change(good, message.Quantity);

            //Сохраняем аггрегат
            _basketRepository.Add(basket);
        }
        else
        {
            //Изменяем аггрегат
            basket.Change(good, message.Quantity);

            //Сохраняем аггрегат
            _basketRepository.Update(basket);
        }

        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
using BasketApp.Core.Ports;
using MediatR;
using Primitives;

namespace BasketApp.Core.Application.UseCases.Commands.AddDeliveryPeriod;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepository;

    /// <summary>
    /// Ctr
    /// </summary>
    private Handler()
    {}
    
    /// <summary>
    /// Ctr
    /// </summary>
    public Handler(IUnitOfWork unitOfWork, IBasketRepository basketRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
    }

    public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
    {
        //Восстанавливаем аггрегат
        var basket = await _basketRepository.GetAsync(message.BasketId);

        //Изменяем аггрегат
        var timeSlotFromNameResult = Domain.BasketAggregate.TimeSlot.FromName(message.TimeSlot.ToString());
        if (timeSlotFromNameResult.IsFailure) return false;

        var timeSlot = timeSlotFromNameResult.Value;
        var basketAddTimeSlotResult = basket.AddTimeSlot(timeSlot);
        if (basketAddTimeSlotResult.IsFailure) return false;

        //Сохраняем аггрегат
        _basketRepository.Update(basket);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
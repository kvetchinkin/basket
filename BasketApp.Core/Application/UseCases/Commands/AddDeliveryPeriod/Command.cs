using MediatR;

namespace BasketApp.Core.Application.UseCases.Commands.AddDeliveryPeriod;

/// <summary>
/// Добавить период доставки
/// </summary>
public class Command : IRequest<bool>
{
    /// <summary>
    /// Идентификатор корзины
    /// </summary>
    public Guid BasketId { get; }

    /// <summary>
    /// Период доставки
    /// </summary>
    public TimeSlot TimeSlot { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Command()
    {}
    
    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param> 
    /// <param name="timeSlot">Период доставки</param>
    public Command(Guid basketId, TimeSlot timeSlot)
    {
        if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
        if (timeSlot == TimeSlot.None) throw new ArgumentException(nameof(TimeSlot));

        BasketId = basketId;
        TimeSlot = timeSlot;
    }
}

/// <summary>
/// Период доставки
/// </summary>
public enum TimeSlot
{
    None,
    Morning,
    Midday,
    Evening,
    Night
}
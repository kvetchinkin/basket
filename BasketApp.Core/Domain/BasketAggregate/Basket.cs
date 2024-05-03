using BasketApp.Core.Domain.BasketAggregate.DomainEvents;
using BasketApp.Core.Domain.GoodAggregate;
using BasketApp.Core.Domain.SharedKernel;
using CSharpFunctionalExtensions;
using Primitives;

namespace BasketApp.Core.Domain.BasketAggregate;

/// <summary>
///     Корзина
/// </summary>
public class Basket : Aggregate
{
    public static class Errors
    {
        public static Error BasketHasAlreadyBeenIssued()
        {
            return new($"{nameof(Basket).ToLowerInvariant()}.has.already.been.issued", "Корзина уже оформлена");
        }
    }

    /// <summary>
    /// Адрес доставки
    /// </summary>
    public  Address Address { get; private set; }

    /// <summary>
    /// Период доставки
    /// </summary>
    public  TimeSlot TimeSlot { get; private set; }

    /// <summary>
    ///     Товарные позиции
    /// </summary>
    public  List<Item> Items { get; set; } = new();

    /// <summary>
    /// Статус
    /// </summary>
    public  Status Status { get; private set; }

    /// <summary>
    /// Итоговая стоимость корзины, учитывая скидку
    /// </summary>
    public  decimal Total { get; private set; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Basket()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    private Basket(Guid buyerId)
    {
        Id = buyerId;
        Status = Status.Created;
    }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="buyerId">Идентификатор покупателя</param>
    /// <returns>Результат</returns>
    public static Result<Basket, Error> Create(Guid buyerId)
    {
        if (buyerId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(buyerId));
        return new Basket(buyerId);
    }

    /// <summary>
    /// Изменить позицию
    /// </summary>
    /// <param name="good">Товар</param>
    /// <param name="quantity">Количество</param>
    /// <returns>Результат</returns>
    public Result<object, Error> Change(Good good, int quantity)
    {
        if (Status == Status.Confirmed) return Errors.BasketHasAlreadyBeenIssued();
        if (good == null) return GeneralErrors.ValueIsRequired(nameof(good));
        if (quantity < 0) return GeneralErrors.ValueIsInvalid(nameof(quantity));

        var item = Items.SingleOrDefault(x => x.GoodId == good.Id);
        if (item != null)
        {
            if (quantity == 0)
            {
                Items.Remove(item);
            }
            else
            {
                item.SetQuantity(quantity);
            }
        }
        else
        {
            var result = Item.Create(good, quantity);
            if (result.IsFailure)
            {
                return result.Error;
            }

            Items.Add(result.Value);
        }

        return new object();
    }

    /// <summary>
    /// Добавить адрес доставки
    /// </summary>
    /// <param name="address">Адрес доставки</param>
    /// <returns>Результат</returns>
    public Result<object, Error> AddAddress(Address address)
    {
        if (Status == Status.Confirmed) return Errors.BasketHasAlreadyBeenIssued();
        Address = address;
        return new object();
    }

    /// <summary>
    /// Добавить период доставки
    /// </summary>
    /// <param name="timeSlot">Период доставки</param>
    /// <returns>Результат</returns>
    public Result<object, Error> AddTimeSlot(TimeSlot timeSlot)
    {
        if (Status == Status.Confirmed) return Errors.BasketHasAlreadyBeenIssued();
        TimeSlot = timeSlot;
        return new object();
    }

    /// <summary>
    /// Оформить корзину
    /// </summary>
    /// <param name="discount">Скидка</param>
    /// <returns>Результат</returns>
    public Result<object, Error> Checkout(double discount)
    {
        if (Status == Status.Confirmed) return Errors.BasketHasAlreadyBeenIssued();
        if (Items.Count <= 0) return GeneralErrors.InvalidLength(nameof(Items));
        if (Address == null) return GeneralErrors.ValueIsRequired(nameof(Address));
        if (TimeSlot == null) return GeneralErrors.ValueIsRequired(nameof(TimeSlot));
        if (discount is < 0 or > 1) return GeneralErrors.ValueIsInvalid(nameof(discount));

        //Рассчитываем итоговую стоимость, учитывая размер скидки
        var sum = Items.Sum(o => o.GetTotal().Value);
        Total = sum - (sum * (decimal) discount);

        //Меняем статус
        Status = Status.Confirmed;
        
        //Публикуем доменное событие
        RaiseDomainEvent(new BasketConfirmedDomainEvent(Id, Address.Street, Items.Count));
        return new object();
    }
}
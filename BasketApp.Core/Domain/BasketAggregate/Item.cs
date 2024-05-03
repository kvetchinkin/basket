using BasketApp.Core.Domain.GoodAggregate;
using CSharpFunctionalExtensions;
using Primitives;

namespace BasketApp.Core.Domain.BasketAggregate;

/// <summary>
/// Товарная позиция
/// </summary>
public class Item : Entity<Guid>
{
    /// <summary>
    /// Идентификтор товара
    /// </summary>
    public  Guid GoodId { get; private set; }

    /// <summary>
    /// Количество
    /// </summary>
    public  int Quantity { get; private set; }

    /// <summary>
    /// Название
    /// </summary>
    public  string Title { get; private set; }

    /// <summary>
    /// Описание
    /// </summary>
    public  string Description { get; private set; }

    /// <summary>
    /// Стоимость
    /// </summary>
    public  decimal Price { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Item()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="good">Товар</param>
    /// <param name="quantity">Количество</param>
    private Item(Good good, int quantity) : this()
    {
        Id = Guid.NewGuid();
        GoodId = good.Id;
        Title = good.Title;
        Description = good.Description;
        Price = good.Price;
        Quantity = quantity;
    }

    /// <summary>
    /// Factory Method
    /// </summary>
    /// <param name="good">Товар</param>
    /// <param name="quantity">Количество</param>
    /// <returns>Результат</returns>
    public static Result<Item, Error> Create(Good good, int quantity)
    {
        if (good == null) return GeneralErrors.ValueIsRequired(nameof(good));
        if (quantity <= 0) return GeneralErrors.ValueIsInvalid(nameof(quantity));

        return new Item(good, quantity);
    }

    /// <summary>
    /// Изменить количество
    /// </summary>
    /// <param name="quantity">Количество</param>
    /// <returns>Результат</returns>
    public Result<object, Error> SetQuantity(int quantity)
    {
        if (quantity <= 0) return GeneralErrors.ValueIsInvalid(nameof(quantity));
        Quantity = quantity;
        return new object();
    }

    /// <summary>
    /// Рассчитать стоимость позиции
    /// </summary>
    /// <returns>Стоимость</returns>
    public Result<decimal, Error> GetTotal()
    {
        var total = Quantity * Price;
        return total;
    }
}
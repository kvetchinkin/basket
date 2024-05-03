using BasketApp.Core.Domain.SharedKernel;
using CSharpFunctionalExtensions;
using Primitives;

namespace BasketApp.Core.Domain.GoodAggregate;

/// <summary>
/// Товар
/// </summary>
public class Good : Aggregate
{
    /// <summary>
    /// Хлеб
    /// </summary>
    public static readonly Good Bread = new Good(new Guid("ec85ceee-f186-4e9c-a4dd-2929e69e586c")
        , "Хлеб"
        , "Описание хлеба"
        , 100
        , Weight.Create(6).Value);

    /// <summary>
    /// Молоко
    /// </summary>
    public static readonly Good Milk = new Good(new Guid("e8cb8a0b-d302-485a-801c-5fb50aced4d5")
        , "Молоко"
        , "Описание молока"
        , 200
        , Weight.Create(9).Value);

    /// <summary>
    /// Яйца
    /// </summary>
    public static readonly Good Eggs = new Good(new Guid("a1d48be9-4c98-4371-97c0-064bde03874e")
        , "Яйца"
        , "Описание яиц"
        , 300
        , Weight.Create(8).Value);

    /// <summary>
    /// Колбаса
    /// </summary>
    public static readonly Good Sausage = new Good(new Guid("34b1e64a-6471-44a0-8c4a-e5d21584a76c")
        , "Колбаса"
        , "Описание колбасы"
        , 400
        , Weight.Create(4).Value);

    /// <summary>
    /// Кофе
    /// </summary>
    public static readonly Good Coffee = new Good(new Guid("292dc3c5-2bdd-4e0c-bd75-c5e8b07a8792")
        , "Кофе"
        , "Описание кофе"
        , 500
        , Weight.Create(7).Value);

    public static readonly Good Sugar = new Good(new Guid("a3fcc8e1-d2a3-4bd6-9421-c82019e21c2d")
        , "Сахар",
        "Описание сахара"
        , 600
        , Weight.Create(1).Value);

    /// <summary>
    /// Название
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Стоимость
    /// </summary>
    public  decimal Price { get; private set; }

    /// <summary>
    /// Вес
    /// </summary>
    public Weight Weight { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Good()
    {
    }

    private Good(Guid id, string title, string description, decimal price, Weight weight)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Price = price;
        Weight = weight;
    }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="title">Название</param>
    /// <param name="description">Описание</param>
    /// <param name="price">Цена</param>
    /// <param name="weight">Вес</param>
    /// <returns>Результат</returns>
    public static Result<Good, Error> Create(string title, string description, decimal price, Weight weight)
    {
        var id = Guid.NewGuid();
        if (string.IsNullOrEmpty(title)) GeneralErrors.ValueIsInvalid(nameof(title));
        if (string.IsNullOrEmpty(description)) GeneralErrors.ValueIsRequired(nameof(description));
        if (price <= 0) GeneralErrors.ValueIsRequired(nameof(price));
        if (weight == null) GeneralErrors.ValueIsRequired(nameof(weight));

        return new Good(id, title, description, price, weight);
    }

    public static IEnumerable<Good> List()
    {
        yield return Bread;
        yield return Milk;
        yield return Eggs;
        yield return Sausage;
        yield return Coffee;
        yield return Sugar;
    }
}
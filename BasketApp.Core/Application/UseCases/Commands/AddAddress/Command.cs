using MediatR;

namespace BasketApp.Core.Application.UseCases.Commands.AddAddress;

/// <summary>
/// ИДобавить адрес доставки
/// </summary>
public class Command : IRequest<bool>
{
    /// <summary>
    /// Идентификатор корзины
    /// </summary>
    public Guid BasketId { get; }

    // <summary>
    ///     Страна
    /// </summary>
    public string Country { get; }

    /// <summary>
    ///     Город
    /// </summary>
    public string City { get; }

    /// <summary>
    ///     Улица
    /// </summary>
    public string Street { get; }

    /// <summary>
    ///     Дом
    /// </summary>
    public string House { get; }

    /// <summary>
    ///     Квартира
    /// </summary>
    public string Apartment { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Command()
    {}
    
    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param> 
    /// <param name="country">Страна</param>
    /// <param name="city">Город</param>
    /// <param name="street">Улица</param>
    /// <param name="house">Дом</param>
    /// <param name="apartment">Квартира</param>
    public Command(Guid basketId, string country, string city, string street, string house, string apartment)
    {
        if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException(nameof(country));
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException(nameof(city));
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException(nameof(street));
        if (string.IsNullOrWhiteSpace(house)) throw new ArgumentException(nameof(house));
        if (string.IsNullOrWhiteSpace(apartment)) throw new ArgumentException(nameof(apartment));

        BasketId = basketId;
        Country = country;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }
}
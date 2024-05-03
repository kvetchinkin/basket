namespace BasketApp.Core.Application.UseCases.Queries.GetBasket;

public class Response
{
    public Basket Basket { get; set; }

    private Response()
    { }
    
    public Response(Basket basket)
    {
        Basket = basket;
    }
}

public class Basket
{
    public Guid Id { get; set; }

    public Address Address { get; set; }

    public List<Item> Items { get; set; }

    private Basket()
    { }

    public Basket(Guid id, Address address, List<Item> items) : this()
    {
        Id = id;
        Address = address;
        Items = items;
    }
}

public class Address
{
    private Address()
    {
    }

    /// <summary>
    ///     Ctr
    /// </summary>
    public Address(string country, string city, string street, string house, string apartment) : this()
    {
        Country = country;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }

    /// <summary>
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
}

public class Item
{
    /// <summary>
    /// Товар
    /// </summary>
    public Guid GoodId { get; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Item()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    public Item(Guid goodId, int quantity) : this()
    {
        GoodId = goodId;
        Quantity = quantity;
    }
}
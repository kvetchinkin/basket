using Dapper;
using MediatR;
using Npgsql;

namespace BasketApp.Core.Application.UseCases.Queries.GetBasket;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly string _connectionString;

    /// <summary>
    /// Ctr
    /// </summary>
    private Handler()
    {}
    
    public Handler(string connectionString)
    {
        _connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<Response> Handle(Query message, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<dynamic>(
            @"SELECT *
                    FROM public.baskets as b 
                    INNER JOIN public.items as i on b.id=i.basket_id
                    WHERE b.id=@id;"
            , new {id = message.BasketId});

        if (result.AsList().Count == 0)
            return null;

        return new Response(MapBasket(result));
    }

    private Basket MapBasket(dynamic result)
    {
        var address = new Address(
            result[0].address_country,
            result[0].address_city,
            result[0].address_street,
            result[0].address_house,
            result[0].address_apartment);

        var items = new List<Item>();
        foreach (dynamic dItem in result)
        {
            var item = new Item(dItem.good_id, dItem.quantity);
            items.Add(item);
        }

        var basket = new Basket(result[0].id, address, items);
        return basket;
    }
}
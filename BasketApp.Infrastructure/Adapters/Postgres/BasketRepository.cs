using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Infrastructure.Adapters.Postgres;

public class BasketRepository : IBasketRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BasketRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Basket Add(Basket basket)
    {
        if (basket.TimeSlot != null)
        {
            _dbContext.Attach(basket.TimeSlot);
        }

        return _dbContext.Baskets.Add(basket).Entity;
    }

    public void Update(Basket basket)
    {
        if (basket.TimeSlot != null)
        {
            _dbContext.Attach(basket.TimeSlot);
        }

        _dbContext.Entry(basket).State = EntityState.Modified;
    }

    public async Task<Basket> GetAsync(Guid basketId)
    {
        var basket = await _dbContext
            .Baskets
            .Include(x => x.TimeSlot)
            .FirstOrDefaultAsync(o => o.Id == basketId);
        if (basket != null)
        {
            await _dbContext.Entry(basket)
                .Collection(i => i.Items).LoadAsync();
        }
        return basket;
    }
}
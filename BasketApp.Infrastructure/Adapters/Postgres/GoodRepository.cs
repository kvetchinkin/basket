using BasketApp.Core.Domain.GoodAggregate;
using BasketApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Infrastructure.Adapters.Postgres;

public class GoodRepository : IGoodRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public GoodRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Good Add(Good good)
    {
        return _dbContext.Goods.Add(good).Entity;
    }

    public async Task<Good> GetAsync(Guid goodId)
    {
        var good = await _dbContext
            .Goods
            .FirstOrDefaultAsync(o => o.Id == goodId);
        return good;
    }

    public void Update(Good good)
    {
        _dbContext.Entry(good).State = EntityState.Modified;
    }
}
using BasketApp.Core.Domain.GoodAggregate;
using Primitives;

namespace BasketApp.Core.Ports;

/// <summary>
/// Repository для Aggregate Good
/// </summary>
public interface IGoodRepository : IRepository<Good>
{
    /// <summary>
    /// Добавить новый товар
    /// </summary>
    /// <param name="good">Товар</param>
    /// <returns>Товар</returns>
    Good Add(Good good);

    /// <summary>
    /// Обновить существующий товар
    /// </summary>
    /// <param name="good">Товар</param>
    void Update(Good good);

    /// <summary>
    /// Получить товар по идентификатору
    /// </summary>
    /// <param name="goodId">Идентификатор</param>
    /// <returns>Товар</returns>
    Task<Good> GetAsync(Guid goodId);
}
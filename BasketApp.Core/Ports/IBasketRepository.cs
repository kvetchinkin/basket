using BasketApp.Core.Domain.BasketAggregate;
using Primitives;

namespace BasketApp.Core.Ports;

/// <summary>
/// Repository для Aggregate Basket
/// </summary>
public interface IBasketRepository : IRepository<Basket>
{
    /// <summary>
    /// Добавить новую корзину
    /// </summary>
    /// <param name="basket">Корзина</param>
    /// <returns>Корзина</returns>
    Basket Add(Basket basket);

    /// <summary>
    /// Обновить существующю корзину
    /// </summary>
    /// <param name="basket">Корзина</param>
    void Update(Basket basket);

    /// <summary>
    /// Получить корзину по идентификатору
    /// </summary>
    /// <param name="basketId">Идентификатор</param>
    /// <returns>Корзина</returns>
    Task<Basket> GetAsync(Guid basketId);
}
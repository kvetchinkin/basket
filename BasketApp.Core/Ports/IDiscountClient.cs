using BasketApp.Core.Domain.BasketAggregate;

namespace BasketApp.Core.Ports;

public interface IDiscountClient
{
    /// <summary>
    /// Получить информацию о скидке
    /// </summary>
    Task<double> GetDiscountAsync(Basket basket, CancellationToken cancellationToken);
}
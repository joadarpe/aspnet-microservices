using Basket.API.Entities;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetByUserName(string userName);
        Task<ShoppingCart> Update(ShoppingCart cart);
        Task Delete(string userName);
    }
}

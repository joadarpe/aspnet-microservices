using System.Threading.Tasks;
using Discount.gRPC.Entities;

namespace Discount.gRPC.Repositories
{
    public interface IDiscountRepository
    {
        Task<Coupon> GetByProductName(string productName);
        Task<bool> Create(Coupon coupon);
        Task<bool> Delete(string productName);
        Task<bool> Update(Coupon coupon);
    }
}
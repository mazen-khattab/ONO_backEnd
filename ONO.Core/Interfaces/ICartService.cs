using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Interfaces
{
    public interface ICartService : IServices<UsersCart>
    {
        Task<ResponseInfo> AddToCart(int userId, int productId, int amount);
        Task<ResponseInfo> AddToCartGuest(string userId, int productId, int amount);
        Task<ResponseInfo> IncreaseAmount(int userId, int productId);
        Task<ResponseInfo> IncreaseAmountGuest(string userId, int productId);
        Task<ResponseInfo> DecreaseAmount(int userId, int productId);
        Task<ResponseInfo> DecreaseAmountGuest(string userId, int productId);
        Task<ResponseInfo> Delete(int userId, int productId);
        Task<ResponseInfo> DeleteAll(int userid);
        Task<ResponseInfo> DeleteGuest(string userId, int productId);
        Task<ResponseInfo> DeleteGuestAll(string userId);
        Task<ResponseInfo> CleanupExpiredCarts(DateTime date);
    }
}

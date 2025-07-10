using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class CartService : Services<UserProducts>, ICartService
    {
        readonly IServices<Product> _productService;
        readonly IServices<TemporaryReservation> _guestService;
        public CartService(IServices<Product> productService, IServices<TemporaryReservation> guest, IUnitOfWork unitOfWork, IRepo<UserProducts> repo) : base(unitOfWork, repo)
        {
            _productService = productService;
            _guestService = guest;
        }

        public async Task<ResponseInfo> AddToCart(int userId, int productId, int amount)
        {
            await _unitOfWork.BeginTransactionAsync();

            var product = await _productService.GetAsync(p => p.Id == productId);
            var userProduct = await GetAsync(up => up.ProductID == productId && up.UserId == userId);

            if (product is null)
            {
                await _unitOfWork.RollbackAsync();
                return new()
                {
                    IsSuccess = false,
                    Message = "No product with this Id!"
                };
            }

            if (product.Reserved + amount > product.StockUnit)
            {
                await _unitOfWork.RollbackAsync();
                return new()
                {
                    IsSuccess = false,
                    Message = "No enough items!"
                };
            }

            product.Reserved += amount;

            if (userProduct is { })
            {
                userProduct.ProductAmount += amount;
                await SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new()
                {
                    IsSuccess = true,
                    Message = "The prdouct updated seccessful"
                };
            }

            UserProducts userProducts = new()
            {
                UserId = userId,
                ProductID = productId,
                ProductAmount = amount
            };

            await AddAsync(userProducts);
            await SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return new()
            {
                IsSuccess = true,
                Message = "The prdouct added seccessful"
            };
        }

        public async Task<ResponseInfo> AddToCartGuest(string userId, int productId, int amount)
        {
            await _unitOfWork.BeginTransactionAsync();

            var product = await _productService.GetAsync(p => p.Id == productId);
            var guestProduct = await _guestService.GetAsync(gp => gp.ProductId == productId && gp.UserId == userId);

            if (product is null)
            {
                await _unitOfWork.RollbackAsync();
                return new()
                {
                    IsSuccess = false,
                    Message = "No product with this Id!"
                };
            }

            if (product.Reserved + amount > product.StockUnit)
            {
                await _unitOfWork.RollbackAsync();
                return new()
                {
                    IsSuccess = false,
                    Message = "No enough items!"
                };
            }

            product.Reserved += amount;

            if (guestProduct is { })
            {
                guestProduct.ProductAmount += amount;
                await _guestService.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new()
                {
                    IsSuccess = true,
                    Message = "The prdouct updated seccessful"
                };
            }

            TemporaryReservation temporaryReservation = new()
            {
                UserId = userId,
                ProductId = productId,
                ProductAmount = amount,
            };

            await _guestService.AddAsync(temporaryReservation);
            await _guestService.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return new()
            {
                IsSuccess = true,
                Message = "The prdouct added seccessful"
            };
        }

        public async Task<ResponseInfo> IncreaseAmount(int userId, int productId)
        {
            await _unitOfWork.BeginTransactionAsync();

            var userProduct = await GetAsync(up => up.ProductID == productId && up.UserId == userId);
            var product = await _productService.GetAsync(p => p.Id == productId);

            if (userProduct is null)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "the product does not exist",
                };
            }

            if (product.Reserved + 1 <= product.StockUnit)
            {
                product.Reserved += 1;
                userProduct.ProductAmount += 1;

                await SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new()
                {
                    IsSuccess = true,
                    Message = "the amount has been increased"
                };
            }

            await _unitOfWork.RollbackAsync();
            return new()
            {
                IsSuccess = false,
                Message = "can not increase the amount, no enough items!",
            };
        }

        public async Task<ResponseInfo> DecreaseAmount(int userId, int productId)
        {
            var userProduct = await GetAsync(up => up.ProductID == productId && up.UserId == userId);
            var product = await _productService.GetAsync(p => p.Id == productId);

            if (userProduct is null)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "the product does not exist",
                };
            }

            if (product.Reserved - 1 > 0)
            {
                product.Reserved -= 1;
                userProduct.ProductAmount -= 1;

                await SaveChangesAsync();

                return new()
                {
                    IsSuccess = true,
                    Message = "the amount has been Decreased"
                };
            }

            return new()
            {
                IsSuccess = false,
                Message = "can not decrease the amount!",
            };
        }

        public async Task<ResponseInfo> Delete(int userId, int productId)
        {
            var userProduct = await GetAsync(up => up.ProductID == productId && up.UserId == userId);
            var product = await _productService.GetAsync(p => p.Id == productId);

            if (userProduct is null)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "the product does not exist",
                };
            }

            product.Reserved -= userProduct.ProductAmount;

            await DeleteAsync(userProduct);
            await SaveChangesAsync();

            return new()
            {
                IsSuccess = true,
                Message = "The product has been deleted ✅",
            };
        }

        public async Task<ResponseInfo> DeleteGuest(string userId, int productId)
        {
            var guestProducts = await _guestService.GetAsync(gp => gp.UserId == userId && gp.ProductId == productId);
            var product = await _productService.GetAsync(p => p.Id == productId);

            if (guestProducts is null)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "the product does not exist",
                };
            }

            product.Reserved -= guestProducts.ProductAmount;

            await _guestService.DeleteAsync(guestProducts);
            await SaveChangesAsync();

            return new()
            {
                IsSuccess = true,
                Message = "The product has been deleted ✅",
            };
        }
    }
}

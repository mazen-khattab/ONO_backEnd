using Microsoft.Extensions.Logging;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.Interfaces;
using ONO.Core.Entities;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Decorator.CacheDecorator
{
    public class CacheProductDecorator : IProductServices
    {
        readonly IProductServices _productServices;
        readonly ICacheServices _cache;
        readonly ILogger<CacheProductDecorator> _logger;

        public CacheProductDecorator(IProductServices productServices, ICacheServices cache, ILogger<CacheProductDecorator> logger)
        {
            _productServices = productServices;
            _cache = cache;
            _logger = logger;
        }


        public async Task<(IEnumerable<Product>, int)> GetAllAsync(Expression<Func<Product, bool>> filter = null, bool tracked = true, int pageNumber = 0, int pageSize = 0, params Expression<Func<Product, object>>[] includes)
        {
            // i cannot add a key to the request that has a filter or includes
            if (filter is { } || (includes is { } && includes.Count() > 0))
            {
                _logger.LogInformation("cannot return the products from the cache because there is no a key");
                return await _productServices.GetAllAsync(filter, tracked, pageNumber, pageSize, includes);
            }

            string key = $"allProducts-{tracked}-{pageNumber}-{pageSize}";

            return await _cache.GetOrCreateAsync(key, () => _productServices.GetAllAsync(filter, tracked, pageNumber, pageSize, includes));
        }

        public async Task<(IEnumerable<ProductDto>, int)> GetAllProducts(ProductRequestDto productDto)
        {
            return await GetAllProducts(productDto);
        }

        public async Task<Product> GetAsync(Expression<Func<Product, bool>> filter = null, bool tracked = true, params Expression<Func<Product, object>>[] includes)
        {
            // cannot cache the product returned because there is no unique key for each product returned
            return await _productServices.GetAsync(filter, tracked, includes);
        }

        public async Task AddAsync(Product entity)
        {
            await _productServices.AddAsync(entity);
            await _cache.RemovebyPrefixAsync("allProducts");
        }
         
        public async Task DeleteAsync(Product entity)
        {
            await _productServices.DeleteAsync(entity);
            await _cache.RemovebyPrefixAsync("allProducts");
        }

        public async Task UpdateAsync(Product entity)
        {
            await _productServices.UpdateAsync(entity);
            await _cache.RemovebyPrefixAsync("allProducts");
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _productServices.SaveChangesAsync();
        }
    }
}

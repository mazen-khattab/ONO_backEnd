using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ONO.Core.Interfaces;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class CacheServices : ICacheServices
    {
        readonly IMemoryCache _memoryCache;
        readonly ILogger<CacheServices> _logger;
        readonly ConcurrentDictionary<string, byte> _keys = new();

        public CacheServices(IMemoryCache cache, ILogger<CacheServices> logger) => (_memoryCache, _logger) = (cache, logger);

        private void CheckIfKeyIsNull(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException($"{key}");
            }
        }

        public Task<(T? value, bool found)> GetAsync<T>(string key)
        {
            _logger.LogInformation("Get cache for key: {key}", key);
            try
            {
                CheckIfKeyIsNull(key);

                if (_memoryCache.TryGetValue(key, out T value))
                {
                    _logger.LogInformation("Cache hit for key: {key}", key);
                    return Task.FromResult((value, true));
                }

                _logger.LogInformation("Cache miss for key: {key}", key);
                return Task.FromResult<(T?, bool)>((default, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for {key}: ", key);
                return Task.FromException<(T?, bool)>(ex);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            _logger.LogInformation("Set the cache for key: {key}", key);
            try
            {
                CheckIfKeyIsNull(key);

                MemoryCacheEntryOptions cacheOptions = new();

                if (expiration.HasValue)
                {
                    cacheOptions.SetAbsoluteExpiration(expiration.Value);
                }
                else
                {
                    cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(2));
                }

                _memoryCache.Set(key, value, cacheOptions);
                _keys.TryAdd(key, 0);
                _logger.LogInformation("Cached value for key: {key}", key);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for {key}: ", key);
                return Task.FromException(ex);
            }
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            _logger.LogInformation("Get or create the cache for {key}", key);
            try
            {
                CheckIfKeyIsNull(key);

                var result = await GetAsync<T>(key);

                if (result.found)
                {
                    return result.value;
                }
                else
                {
                    var value = await factory();

                    if (value is not null)
                    {
                        await SetAsync(key, value, expiration);
                    }

                    return value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for {key}: ", key);
                throw;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _keys.TryRemove(key, out _);

                _logger.LogInformation("Removed cache for {key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for {key}: ", key);
                return Task.FromException(ex);
            }
        }

        public Task RemovebyPrefixAsync(string prefixkey)
        {
            try
            {
                var keysToRemove = _keys.Keys.Where(k => k.StartsWith(prefixkey)).ToList();

                foreach (var key in keysToRemove)
                {
                    RemoveAsync(key);
                }

                _logger.LogInformation("Removed {count} cache entries with prefix: {prefixKey}", keysToRemove.Count, prefixkey);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by prefix {key}: ", prefixkey);
                return Task.FromException(ex);
            }
        }
    }
}

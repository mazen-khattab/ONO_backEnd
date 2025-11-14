using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Interfaces
{
    public interface ICacheServices
    {
        Task<(T? value, bool found)> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemovebyPrefixAsync(string prefixKey);
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    }
}

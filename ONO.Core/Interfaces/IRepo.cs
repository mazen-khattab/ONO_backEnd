using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Interfaces
{
    public interface IRepo<T> where T : class
    {
        Task<(IEnumerable<T>, int)> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, int pageNumber = 0, int pageSize = 0, params Expression<Func<T, object>>[] includes);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}

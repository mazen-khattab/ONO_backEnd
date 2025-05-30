using Microsoft.EntityFrameworkCore;
using ONO.Core.Interfaces;
using ONO.Infrasturcture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Repositories
{
    public class Repo<T> : IRepo<T> where T : class
    {
        readonly AppDbContext _context;
        readonly DbSet<T> _dbSet;

        public Repo(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 0, params Expression<Func<T, object>>[] includes)
        {
            var query = tracked? _dbSet.AsQueryable() : _dbSet.AsNoTracking().AsQueryable();

            if (filter is { })
            {
                query = query.Where(filter);
            }

            if (includes is { } && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (pageSize > 0 && pageNumber > 0)
            {
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
        {
            var query = await GetAllAsync(filter, tracked, includes: includes);

            return query.FirstOrDefault();
        }

        public Task CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}

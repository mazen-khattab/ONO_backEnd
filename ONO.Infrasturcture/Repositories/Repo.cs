﻿using Microsoft.EntityFrameworkCore;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
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


        public async Task<(IEnumerable<T>, int)> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, int pageNumber = 0, int pageSize = 0, params Expression<Func<T, object>>[] includes)
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

            int count = await query.CountAsync();


            if (pageSize > 0 && pageNumber > 0)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            var items = await query.ToListAsync();

            return (items, count);
        }

        public async Task<int> GetCount() => await _dbSet.CountAsync();

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
        {
            var (items, count) = await GetAllAsync(filter, tracked, includes: includes);

            return items.FirstOrDefault();
        }

        public async Task CreateAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task RemoveAsync(T entity) => _dbSet.Remove(entity);

        public async Task UpdateAsync(T entity) => _dbSet.Update(entity);
    }
}

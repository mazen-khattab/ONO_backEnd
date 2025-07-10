using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ONO.Core.Interfaces;
using ONO.Infrasturcture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly AppDbContext _context;
        IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context) => (_context) = (context);

        public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync() => await _transaction?.CommitAsync();

        public async Task RollbackAsync() => await _transaction?.RollbackAsync();

        public void Dispose() => _context.Dispose();

        public Task<int> SaveChanges() => _context.SaveChangesAsync();
    }
}

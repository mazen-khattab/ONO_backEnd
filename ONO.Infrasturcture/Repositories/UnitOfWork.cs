using Microsoft.EntityFrameworkCore;
using ONO.Core.Interfaces;
using ONO.Infrasturcture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Repositories
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        readonly AppDbContext _context;
        public IRepo<T> Repo { get; private set; }

        public UnitOfWork(IRepo<T> repo, AppDbContext context) => (Repo, _context) = (repo, context);

        public void Dispose() => _context.Dispose();
        public Task<int> SaveChanges() => _context.SaveChangesAsync();
    }
}

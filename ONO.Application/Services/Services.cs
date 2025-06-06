using ONO.Core.AnotherObjects;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class Services<T> : IServices<T> where T : class
    {
        readonly IUnitOfWork<T> _unitOfWork;

        public Services(IUnitOfWork<T> unitOfWork) => (_unitOfWork) = (unitOfWork);


        public async Task<(IEnumerable<T>, int)> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, int pageNumber = 0, int pageSize = 0, params Expression<Func<T, object>>[] includes) => await _unitOfWork.Repo.GetAllAsync(filter, tracked, pageNumber, pageSize, includes);

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
            => await _unitOfWork.Repo.GetAsync(filter, tracked, includes);

        public async Task<int> GetCount() => await _unitOfWork.Repo.GetCount();

        public async Task AddAsync(T entity)
        {
            await _unitOfWork.Repo.CreateAsync(entity);
            await _unitOfWork.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            await _unitOfWork.Repo.UpdateAsync(entity);
            await _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            await _unitOfWork.Repo.RemoveAsync(entity);
            await _unitOfWork.SaveChanges();
        }
    }
}

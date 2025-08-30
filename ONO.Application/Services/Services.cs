using ONO.Core.Interfaces;
using System.Linq.Expressions;

namespace ONO.Application.Services
{
    public class Services<T> : IServices<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IRepo<T> _repo;

        public Services(IUnitOfWork unitOfWork, IRepo<T> repo) => (_unitOfWork, _repo) = (unitOfWork, repo);


        public async Task<(IEnumerable<T>, int)> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, int pageNumber = 0, int pageSize = 0, params Expression<Func<T, object>>[] includes) => await _repo.GetAllAsync(filter, tracked, pageNumber, pageSize, includes);

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
            => await _repo.GetAsync(filter, tracked, includes);

        public async Task AddAsync(T entity) => await _repo.CreateAsync(entity);

        public async Task UpdateAsync(T entity) => await _repo.UpdateAsync(entity);

        public async Task DeleteAsync(T entity) => await _repo.RemoveAsync(entity);

        public async Task<int> SaveChangesAsync() => await _unitOfWork.SaveChanges();
    }
}

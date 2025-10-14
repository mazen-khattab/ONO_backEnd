using Microsoft.Extensions.Logging;
using ONO.Core.Interfaces;
using System.Linq.Expressions;

namespace ONO.Application.Services
{
    public class Services<T> : IServices<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IRepo<T> _repo;
        protected readonly ILogger<Services<T>> _logger;

        public Services(IUnitOfWork unitOfWork, IRepo<T> repo, ILogger<Services<T>> logger)
            => (_unitOfWork, _repo, _logger) = (unitOfWork, repo, logger);

        public async Task<(IEnumerable<T>, int)> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, int pageNumber = 0, int pageSize = 0, params Expression<Func<T, object>>[] includes)
        {
            _logger.LogInformation("Getting all {T} from services", typeof(T));
            return await _repo.GetAllAsync(filter, tracked, pageNumber, pageSize, includes);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
        {
            _logger.LogInformation("Getting single entity of type {T} from services", typeof(T));
            return await _repo.GetAsync(filter, tracked, includes);
        }

        public async Task AddAsync(T entity)
        {
            _logger.LogInformation("Adding entity of type {T} from servicse", typeof(T));
            await _repo.CreateAsync(entity);
        }
        public async Task UpdateAsync(T entity)
        {
            _logger.LogInformation("Updating entity of type{T} from services", typeof(T));
            await _repo.UpdateAsync(entity); 
        }

        public async Task DeleteAsync(T entity)
        {
            _logger.LogInformation("Deleting entity of type {T} from services", typeof(T));
            await _repo.RemoveAsync(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger.LogInformation("save chages in services");
            return await _unitOfWork.SaveChanges();
        }
    }
}

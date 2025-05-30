using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Interfaces
{
    public interface IUnitOfWork<T> : IDisposable where T : class
    {   
        IRepo<T> Repo { get; }
        Task<int> SaveChanges();
    }
}

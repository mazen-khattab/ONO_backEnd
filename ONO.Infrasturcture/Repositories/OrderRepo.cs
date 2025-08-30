using Microsoft.EntityFrameworkCore;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using ONO.Infrasturcture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Repositories
{
    public class OrderRepo : Repo<Order>, IOrderRepo
    {
        public OrderRepo(AppDbContext context) : base(context) { }

        public async Task<ICollection<Order>> GetOrderHistory(int userId)
        {
            return await _dbSet.Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }
    }
}

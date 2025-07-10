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
    public class CartRepo : Repo<UserProducts>, ICartRepo
    {
        public CartRepo(AppDbContext context) : base(context) { }
    }
}

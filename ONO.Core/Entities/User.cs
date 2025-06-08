using Microsoft.AspNetCore.Identity;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class User : IdentityUser<int>, ISoftDeleteble
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public bool IsDeleted { get; set; }

        public RefreshToken RefreshToken { get; set; }
        public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public ICollection<UserProducts> UserProducts { get; set; } = new List<UserProducts>();
    }
}

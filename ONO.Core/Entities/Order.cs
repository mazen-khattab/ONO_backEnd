using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class Order : BaseEntity
    {
        public string Status { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int? AddressId { get; set; }
        public int UserId { get; set; }

        public UserAddress Address { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}

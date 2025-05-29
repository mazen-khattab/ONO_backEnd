using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class InventoryTransaction : BaseEntity
    {
        public string TransactionType { get; set; } 
        public double Quantity { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public User User { get; set; } = null!;
        public Order? Order { get; set; }
        public Product Product { get; set; } = null!;
    }
}

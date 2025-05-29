using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class Product : BaseEntity, ISoftDeleteble
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public int StockUnit { get; set; }
        public int? CategoryId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSpecial { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}

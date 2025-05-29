using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public string AltText { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}

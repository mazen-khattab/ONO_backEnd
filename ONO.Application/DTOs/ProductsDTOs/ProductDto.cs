using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.ProductsDTOs
{
    public class ProductDto
    {
        public int ProductAmount { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string? cateName { get; set; }
        public int AgeRange { get; set; }
    }
}

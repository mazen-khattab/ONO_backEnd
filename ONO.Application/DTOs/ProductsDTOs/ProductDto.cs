using ONO.Application.DTOs.ProductImagesDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.ProductsDTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string? cateName { get; set; }
        public int AgeRange { get; set; }
        public int StockUnit { get; set; }
        public int Reserved { get; set; }
        public int ProductAmount { get; set; }
        public ICollection<ProductImagesDto> Gallary { get; set; } = new List<ProductImagesDto>();
    }
}

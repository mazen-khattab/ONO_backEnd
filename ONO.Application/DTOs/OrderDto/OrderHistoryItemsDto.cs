using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.OrderDto
{
    public class OrderHistoryItemsDto
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
    }
}

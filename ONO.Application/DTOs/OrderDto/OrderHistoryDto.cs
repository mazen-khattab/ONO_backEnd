using ONO.Application.DTOs.ProductsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.OrderDto
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<OrderHistoryItemsDto> Products { get; set; }
    }
}

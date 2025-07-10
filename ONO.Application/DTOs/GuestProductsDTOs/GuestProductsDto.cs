using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.GuestProductsDTOs
{
    public class GuestProductsDto
    {
        public string Name { get; set; }
        public int ageRange { get; set; }
        public string cateName { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public decimal price { get; set; }
        public int StockUnit { get; set; }
        public int Reserved { get; set; }
        public int ProductAmount { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
}

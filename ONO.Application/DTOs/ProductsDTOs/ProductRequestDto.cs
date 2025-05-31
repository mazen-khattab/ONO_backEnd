using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.ProductsDTOs
{
    public class ProductRequestDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? CateName { get; set; }
        public int AgeRange { get; set; }
    }
}

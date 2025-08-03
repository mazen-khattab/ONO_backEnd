using ONO.Application.DTOs.UserDTOs;
using ONO.Application.DTOs.UserProductsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.OrderDto
{
    public class OrderInfoDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public UserAddressDto Address { get; set; }
        public List<UserProductsDTO> CartItems { get; set; }
    }
}

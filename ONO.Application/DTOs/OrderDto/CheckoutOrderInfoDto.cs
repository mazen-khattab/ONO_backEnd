using ONO.Application.DTOs.UserDTOs;
using ONO.Application.DTOs.UsersCartDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.OrderDto
{
    public class CheckoutOrderInfoDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public UserAddressDto Address { get; set; }
        public List<UsersCartDTO> CartItems { get; set; }
    }
}

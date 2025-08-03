using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.UserDTOs
{
    public class UserAddressDto
    {
        public string Governorate { get; set; }
        public string City { get; set; }
        public string FullAddress { get; set; }
    }
}

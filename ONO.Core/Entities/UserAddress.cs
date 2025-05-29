using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class UserAddress : BaseEntity
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
